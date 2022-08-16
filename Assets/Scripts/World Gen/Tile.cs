using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Tile : MonoBehaviour
{
    [SerializeField] public TileInfo tileInfo;
    [SerializeField] public ItemInfo itemInfo;
    [SerializeField] public GameObject currentPlacedItem;
    [SerializeField] public List<ItemInfo> currentPlacedResources = new List<ItemInfo>();
    [SerializeField] private GameObject workerItemPrefab;
    [SerializeField] private GameObject knightItemPrefab;
    [SerializeField] private GameObject archerItemPrefab;
    [SerializeField] public Transform[] workerPoints;
    [SerializeField] public Transform[] resourcePoints;
    [SerializeField] private GameObject buildingPlacementSmokePrefab;

    public bool isOccupiedWithBuilding = false;
    public bool isOccupiedWithWorkers = false;
    public bool isOccupiedWithResources = false;

    private CraftingManager craftingManager;
    private HighlightedBorder highlightedBorder;

    private void Awake() {
        highlightedBorder = FindObjectOfType<HighlightedBorder>();
        craftingManager = GetComponent<CraftingManager>();
    } 

    public void UpdateCurrentPlacedItem(ItemInfo itemInfo, GameObject thisPlacedItem) {
        this.itemInfo = itemInfo;
        currentPlacedItem = thisPlacedItem;
        currentPlacedResources.Add(itemInfo);
        currentPlacedItem.GetComponent<PlacedItem>().CheckForValidRecipe();

    }

    public void UpdateCurrentPlacedResourceList(ItemInfo itemInfo) {
        currentPlacedResources.Add(itemInfo);
        resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
    }


    public void InstantiateSmokePrefab() {
        GameObject smokePrefab = Instantiate(buildingPlacementSmokePrefab, transform.position, transform.rotation);
        StartCoroutine(DestroySmokeCo(smokePrefab));
    }

    private IEnumerator DestroySmokeCo(GameObject smokePrefab) {
        yield return new WaitForSeconds(3f);
        Destroy(smokePrefab);
    }

    public bool PlaceWorker(GameObject workerPrefab, int currentHealth, int currentStrength, int currentFoodNeeded) {
        foreach (var worker in workerPoints)
        {
            if (worker.childCount == 0) {
                GameObject newWorker = Instantiate(workerPrefab, worker.position, transform.rotation);
                newWorker.transform.parent = worker;
                newWorker.GetComponent<Worker>().TransferHealth(currentHealth);
                newWorker.GetComponent<Worker>().TransferStrength(currentStrength, currentFoodNeeded);
                isOccupiedWithWorkers = true;
                GetComponent<CraftingManager>().hasWorkers = true;
                
                if (GetComponent<CraftingManager>().isCrafting) {
                    newWorker.GetComponent<Worker>().StartWorking();
                }
                AudioManager.instance.Play("Click");
                return true;
            }
        }

        return false;
    }

    public bool PlaceKnight(GameObject knightPrefab, int currentHealth, int currentStrength, int foodNeeded) {
        foreach (var worker in workerPoints)
        {
            if (worker.childCount == 0) {
                GameObject newKnight = Instantiate(knightPrefab, worker.position, transform.rotation);
                newKnight.transform.parent = worker;
                newKnight.GetComponent<Knight>().TransferHealth(currentHealth);
                newKnight.GetComponent<Knight>().TransferStrength(currentStrength, foodNeeded);
                isOccupiedWithWorkers = true;
                AudioManager.instance.Play("Click");
                return true;
            }
        }

        return false;
    }

    public bool PlaceArcher(GameObject archerPrefab, int currentHealth, int currentStrength, int currentFoodNeeded) {
        foreach (var worker in workerPoints)
        {
            if (worker.childCount == 0) {
                GameObject newArcher = Instantiate(archerPrefab, worker.position, transform.rotation);
                newArcher.transform.parent = worker;
                newArcher.GetComponent<Archer>().TransferHealth(currentHealth);
                newArcher.GetComponent<Archer>().TransferStrength(currentStrength, currentFoodNeeded);
                isOccupiedWithWorkers = true;
                AudioManager.instance.Play("Click");
                return true;
            }
        }

        return false;
    }

    public bool PlaceResource(GameObject itemPrefab) {
        foreach (var resource in resourcePoints)
        {
            if (resource.childCount == 0) {
                GameObject newResource = Instantiate(itemPrefab, resource.position, transform.rotation);
                newResource.transform.parent = resource;
                isOccupiedWithResources = true;
                AudioManager.instance.Play("Click");
                resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
                return true;
            }

        }

        return false;
    }

    public void PluckItemsOffTile() {
        
        if (currentPlacedItem && currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary == false) {
            Destroy(currentPlacedItem);
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);

            GameObject newObject = Instantiate(itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
            newObject.GetComponent<DraggableItem>().UpdateAmountLeftToHarvest(GetComponent<CraftingManager>().amountLeftToCraft);
            PopTileCleanUp();
        }

        if (currentPlacedItem && currentPlacedItem.GetComponent<Furnace>()) {
            currentPlacedItem.GetComponent<Furnace>().AbandonSmelting();
        }

        if (currentPlacedItem && currentPlacedItem.GetComponent<House>()) {
            currentPlacedItem.GetComponent<House>().StopBabyMaking();
        }

        foreach (var worker in workerPoints)
        {

            if (worker.childCount == 1) {
                if (worker.GetChild(0).GetComponent<Worker>()) {
                    Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                    GameObject newWorker = Instantiate(workerItemPrefab, spawnItemsVector3, transform.rotation);
                    newWorker.GetComponent<Worker>().TransferHealth(worker.GetChild(0).GetComponent<Worker>().myHealth);
                    newWorker.GetComponent<Worker>().TransferStrength(worker.GetChild(0).GetComponent<Worker>().myWorkingStrength, worker.GetChild(0).GetComponent<Worker>().foodNeededToUpPickaxeStrengthCurrent);
                }

                if (worker.GetChild(0).GetComponent<Knight>()) {
                    Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                    GameObject newKnight = Instantiate(knightItemPrefab, spawnItemsVector3, transform.rotation);
                    newKnight.GetComponent<Knight>().TransferHealth(worker.GetChild(0).GetComponent<Knight>().myHealth);
                    newKnight.GetComponent<Knight>().TransferStrength(worker.GetChild(0).GetComponent<Knight>().myWorkingStrength, worker.GetChild(0).GetComponent<Knight>().foodNeededToUpCombatValue);
                }

                if (worker.GetChild(0).GetComponent<Archer>()) {
                    Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                    GameObject newArcher = Instantiate(archerItemPrefab, spawnItemsVector3, transform.rotation);
                    newArcher.GetComponent<Archer>().TransferHealth(worker.GetChild(0).GetComponent<Archer>().myHealth);
                    newArcher.GetComponent<Archer>().TransferStrength(worker.GetChild(0).GetComponent<Archer>().myWorkingStrength, worker.GetChild(0).GetComponent<Archer>().foodNeededToUpCombatValue);
                }

                Destroy(worker.GetChild(0).transform.gameObject);
                PopTileCleanUp();
            }
        }

        foreach (var resource in resourcePoints)
        {
            if (resource.childCount == 1) {
                Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                Instantiate(resource.GetChild(0).GetComponent<PlacedItem>().itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
                Destroy(resource.GetChild(0).transform.gameObject);
                PopTileCleanUp();
            }
        }
    }

    private void PopTileCleanUp() {

        isOccupiedWithWorkers = false;
        isOccupiedWithResources = false;

        if (currentPlacedItem) {
            if (currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary == false) {
                isOccupiedWithBuilding = false;
            }

            if (!currentPlacedItem.GetComponent<UnlimitedHarvest>() && !currentPlacedItem.GetComponent<OrcRelic>()) {
                craftingManager.DoneCrafting();
            }
        } else {
            craftingManager.DoneCrafting();
        }

        craftingManager.WorkerCountToZero();
    }

    public void DoneCraftingDestroyItem() {

        if (resourcePoints[0].childCount > 0) {
            foreach (var resource in resourcePoints)
            {
                if (resource.childCount > 0) {
                    Destroy(resource.GetChild(0).gameObject);
                }
            }
        }

        isOccupiedWithResources = false;

        if (currentPlacedItem) {
            if (currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary == false || currentPlacedItem.GetComponent<OrcRelic>()) {
                isOccupiedWithBuilding = false;
                Destroy(currentPlacedItem);
            }
        }
    }
}
