using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public TileInfo tileInfo;
    [SerializeField] public ItemInfo itemInfo;
    [SerializeField] private GameObject currentPlacedItem;
    [SerializeField] private GameObject workerItemPrefab;
    [SerializeField] private Transform[] workerPoints;

    private GameObject tileHighlight;

    public bool isOccupiedWithItem = false;
    public bool isOccupiedWithWorkers = false;

    private CraftingManager craftingManager;

    private void Awake() {
        craftingManager = GetComponent<CraftingManager>();
        tileHighlight = GameObject.Find("Highlighted Border");
    }

    public void UpdateCurrentPlacedItem(ItemInfo itemInfo, GameObject thisPlacedItem) {
        this.itemInfo = itemInfo;
        currentPlacedItem = thisPlacedItem;
        currentPlacedItem.GetComponent<PlacedItem>().CheckForValidRecipe();
    }

    private void OnMouseOver() {
        if ((isOccupiedWithItem || isOccupiedWithWorkers) && Input.GetMouseButtonDown(1)) {
            PluckItemsOffTile();
        }

        tileHighlight.GetComponent<SpriteRenderer>().enabled = true;

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
                return true;
            }
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
                Destroy(worker.GetChild(0).transform.gameObject);
                Instantiate(workerItemPrefab, spawnItemsVector3, transform.rotation);
            }
        }

        isOccupiedWithItem = false;
        isOccupiedWithWorkers = false;

        craftingManager.DoneCrafting();
        craftingManager.WorkerCountToZero();
    }

    public void DoneCraftingDestroyItem() {
        isOccupiedWithItem = false;

        Destroy(currentPlacedItem);
    }
}
