using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        NewGame();
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    public void NewGame() {
        this.gameData = new GameData();
    }

    public void LoadGame() {
        this.gameData = dataHandler.Load();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        if (this.gameData == null) {
            Debug.Log("No data was found");
        } else {
            LoadData(gameData);

            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }

            this.gameData = new GameData();
        }
        
    }


    public void LoadData(GameData data) {
        LoadTiles.instance.SpawnTiles(data);
        LoadItems.instance.SpawnItems(data);
        LoadClouds.instance.SpawnClouds(data);
        LoadItems.instance.SpawnPlacedItems(data);
        LoadPopulation.instance.SpawnDraggableItemWorkers(data);
        LoadPopulation.instance.SpawnPlacedWorkers(data);
        LoadStorageItems.instance.SpawnStorageItems(data);
    }

    public void SaveGame() {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    public List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
