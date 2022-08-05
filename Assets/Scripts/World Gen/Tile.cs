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
    [SerializeField] public Transform[] workerPoints;
    [SerializeField] public Transform[] resourcePoints;
    private int tileCloudLayerMask;

    private GameObject tileHighlight;

    public bool isOccupiedWithBuilding = false;
    public bool isOccupiedWithWorkers = false;
    public bool isOccupiedWithResources = false;

    private CraftingManager craftingManager;
    private HighlightedBorder highlightedBorder;
    private AudioManager audioManager;

    private void Awake() {
        audioManager = FindObjectOfType<AudioManager>();
        highlightedBorder = FindObjectOfType<HighlightedBorder>();
        craftingManager = GetComponent<CraftingManager>();
        tileHighlight = GameObject.Find("Highlighted Border");
        tileCloudLayerMask = LayerMask.GetMask("Tile");
        tileCloudLayerMask += LayerMask.GetMask("Clouds");

    }

    private void Update() {
        CustomOnMouseOver();
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

    private void CustomOnMouseOver() {

        RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, tileCloudLayerMask);

        if (hit.Length > 0) {
            if (hit[0].transform == transform) {
                if ((isOccupiedWithBuilding || isOccupiedWithWorkers || isOccupiedWithResources) && Input.GetMouseButtonDown(1)) {
                    PluckItemsOffTile();
                }

                tileHighlight.GetComponent<SpriteRenderer>().enabled = true;

                tileHighlight.transform.position = hit[0].transform.position;
            }
        }
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
                audioManager.Play("Click");
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
                audioManager.Play("Click");
                resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
                return true;
            }

        }

        return false;
    }

    private void PluckItemsOffTile() {
        Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
        
        if (currentPlacedItem && currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary == false) {
            Destroy(currentPlacedItem);
            GameObject newObject = Instantiate(itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
            newObject.GetComponent<DraggableItem>().UpdateAmountLeftToHarvest(GetComponent<CraftingManager>().amountLeftToCraft);
            PopTileCleanUp();
        }

        foreach (var worker in workerPoints)
        {
            if (worker.childCount == 1) {
                spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
                Instantiate(workerItemPrefab, spawnItemsVector3, transform.rotation);
                Destroy(worker.GetChild(0).transform.gameObject);
                PopTileCleanUp();
            }
        }

        foreach (var resource in resourcePoints)
        {
            if (resource.childCount == 1) {
                spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
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

            if (!currentPlacedItem.GetComponent<UnlimitedHarvest>()) {
                craftingManager.DoneCrafting();
            }
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
            if (currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary == false) {
                isOccupiedWithBuilding = false;
                Destroy(currentPlacedItem);
            }
        }
    }
}
