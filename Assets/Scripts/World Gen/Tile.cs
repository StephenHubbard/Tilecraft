using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public TileInfo tileInfo;
    [SerializeField] public ItemInfo itemInfo;
    [SerializeField] private GameObject currentPlacedItem;
    [SerializeField] public List<ItemInfo> currentPlacedResources = new List<ItemInfo>();
    [SerializeField] private GameObject workerItemPrefab;
    [SerializeField] public Transform[] workerPoints;
    [SerializeField] public Transform[] resourcePoints;

    private GameObject tileHighlight;

    public bool isOccupiedWithBuilding = false;
    public bool isOccupiedWithWorkers = false;
    public bool isOccupiedWithResources = false;

    private CraftingManager craftingManager;
    private HighlightedBorder highlightedBorder;

    private void Awake() {
        highlightedBorder = FindObjectOfType<HighlightedBorder>();
        craftingManager = GetComponent<CraftingManager>();
        tileHighlight = GameObject.Find("Highlighted Border");
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

    private void OnMouseOver() {
        if ((isOccupiedWithBuilding || isOccupiedWithWorkers || isOccupiedWithResources) && Input.GetMouseButtonDown(1)) {
            PluckItemsOffTile();
        }

        tileHighlight.GetComponent<SpriteRenderer>().enabled = true;

        // buggy not working as intended
        // if (highlightedBorder.currentHeldItem != null) {
        //     highlightedBorder.checkIfHoveredOverTileIsValid(tileInfo);
        // }

        tileHighlight.transform.position = transform.position;
    }

    private void OnMouseExit() {
        tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool PlaceWorker(GameObject workerPrefab) {
        foreach (var worker in workerPoints)
        {
            if (worker.childCount == 0) {
                GameObject newWorker = Instantiate(workerPrefab, worker.position, transform.rotation);
                newWorker.transform.parent = worker;
                isOccupiedWithWorkers = true;
                GetComponent<CraftingManager>().hasWorkers = true;
                if (GetComponent<CraftingManager>().isCrafting) {
                    newWorker.GetComponent<Worker>().StartWorking();
                }
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
                return true;
            }

            // itemPrefab.GetComponent<PlacedItem>().CheckForValidRecipe();
        }

        return false;
    }


    private void PluckItemsOffTile() {
        Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
        
        if (currentPlacedItem) {
            Destroy(currentPlacedItem);
            Instantiate(itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
        }

        foreach (var worker in workerPoints)
        {
            if (worker.childCount == 1) {
                spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                Instantiate(workerItemPrefab, spawnItemsVector3, transform.rotation);
                Destroy(worker.GetChild(0).transform.gameObject);
            }
        }

        foreach (var resource in resourcePoints)
        {
            if (resource.childCount == 1) {
                spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                Instantiate(resource.GetChild(0).GetComponent<PlacedItem>().itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
                Destroy(resource.GetChild(0).transform.gameObject);
            }
        }

        isOccupiedWithBuilding = false;
        isOccupiedWithWorkers = false;
        isOccupiedWithResources = false;

        craftingManager.DoneCrafting();
        craftingManager.WorkerCountToZero();
    }

    public void DoneCraftingDestroyItem() {
        isOccupiedWithBuilding = false;

        if (resourcePoints[0].childCount > 0) {
            foreach (var resource in resourcePoints)
            {
                if (resource.childCount > 0) {
                    Destroy(resource.GetChild(0).gameObject);
                }
            }
        }

        isOccupiedWithResources = false;
        Destroy(currentPlacedItem);
    }
}
