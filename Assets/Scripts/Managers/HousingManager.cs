using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HousingManager : MonoBehaviour
{
    [SerializeField] public int maximumPopulation;
    [SerializeField] public int currentPopulation;
    [SerializeField] private int startingPopulation;
    [SerializeField] private int housePopValue = 5;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text housingText;
    [SerializeField] private Animator housingUIAnimator;
    [SerializeField] private GameObject workerItemPrefab;
    [SerializeField] private Transform storageContainer;

    public static HousingManager instance;

    private void Awake() {
        if (instance == null) {
        instance = this;
        }
    }

    private void Start() {
        DetectHowManyHouses();
        // DetectTotalPopulation();
    }

    private void Update() {
        housingText.text = currentPopulation.ToString() + "/" + maximumPopulation.ToString();
        slider.maxValue = maximumPopulation;
        slider.value = currentPopulation;
    }

    public void DetectTotalPopulation() {
        StartCoroutine(GetAmountOfTotalPopulationEndOfFrameCo());
    }

    public void SpawnStartingThreeWorkers() {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(workerItemPrefab, new Vector3(25.5f, 30, 0), transform.rotation);
        }
    }


    public void DetectHowManyHouses() {
        House[] amountOfHouses = FindObjectsOfType<House>();
        maximumPopulation = (amountOfHouses.Length * housePopValue) + startingPopulation;
    }

    public void BlinkNotEnoughWorkersAnim() {
        housingUIAnimator.SetTrigger("notEnoughHousing");
    }

    public void AddNewWorker() {
        currentPopulation++;
        StartCoroutine(GetAmountOfTotalPopulationEndOfFrameCo());
    }

    public IEnumerator GetAmountOfTotalPopulationEndOfFrameCo() {
        yield return new WaitForEndOfFrame();
        Population[] totalPop = FindObjectsOfType<Population>();

        int amountInStorage = 0;

        foreach (Transform item in storageContainer)
        {
            if (item.GetComponent<StorageItem>().itemInfo.isPopulation) {
                amountInStorage += item.GetComponent<StorageItem>().amountInStorage;
            }
        }

        currentPopulation = totalPop.Length + amountInStorage;

        NotEnoughWorkersCheck();
    }

    private void NotEnoughWorkersCheck() {
        Transform spawnPosition = null;

        House[] houseLengthNullCheck = FindObjectsOfType<House>();

        if (houseLengthNullCheck.Length > 0) {
        spawnPosition = FindObjectOfType<House>().transform;
        }
        
        Tower[] TowerLengthNullCheck = FindObjectsOfType<Tower>();

        if (TowerLengthNullCheck.Length > 0 && spawnPosition == null) {
        spawnPosition = FindObjectOfType<Tower>().transform;
        }

        if (spawnPosition) {
        if (currentPopulation == 1) {
            Instantiate(workerItemPrefab, spawnPosition.position, spawnPosition.rotation);
        }

        if (currentPopulation == 0) {
            Instantiate(workerItemPrefab, spawnPosition.position, spawnPosition.rotation);
            Instantiate(workerItemPrefab, spawnPosition.position, spawnPosition.rotation);
        }
        }
        
    }

    public void AddNewHouse(int amountHouseAdds) {
        maximumPopulation += amountHouseAdds;
    }

    public void AllHousesDetectBabyMaking() {
        StartCoroutine(BabyMakingEndOfFrameCo());
    }

    private IEnumerator BabyMakingEndOfFrameCo() {
        yield return new WaitForEndOfFrame();

        House[] allHouses = FindObjectsOfType<House>();
        foreach (House house in allHouses)
        {
        house.DetectBabyMaking();
        }
    }
}
