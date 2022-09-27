using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToDoList : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string id;
    public ItemInfo itemInfo;

    private void Start() {
        GenerateGuid();
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data) {

    }

    public void SaveData(ref GameData data) {
        if (data.toDoList.ContainsKey(id)) {
            data.toDoList.Remove(id);
        }
        data.toDoList.Add(id, itemInfo);
    }
}
