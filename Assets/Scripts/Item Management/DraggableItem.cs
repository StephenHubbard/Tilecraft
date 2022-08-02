using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [SerializeField] public ItemInfo itemInfo;
    
    private GameObject tileHighlight;
    public Tile currentTile;

    private AudioManager audioManager;

    private void Awake() {
        tileHighlight = GameObject.Find("Highlighted Border");
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start() {
        tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update() {
        if (currentTile != null) {
            tileHighlight.GetComponent<SpriteRenderer>().enabled = true;
            tileHighlight.transform.position = currentTile.transform.position;
        }
    }

    public void setActiveTile(GameObject hitTile) {
        currentTile = hitTile.GetComponent<Tile>();
    }

    public void tileHighlightOff() {
        tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
        currentTile = null;
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.GetComponent<Tile>() && currentTile == other.gameObject.GetComponent<Tile>()) {
            currentTile = null;
            tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void PlaceItemOnTile(int amountInStack) {

        for (int i = amountInStack; i > 0; i--)
        {
            if (itemInfo.name == "Worker") {
                if (currentTile.PlaceWorker(itemInfo.onTilePrefab)) {
                    currentTile.GetComponent<CraftingManager>().IncreaseWorkerCount();
                    if (i == 1) {
                        Destroy(gameObject);
                    }
                    continue;
                } else { 
                    DetermineExtraItems(i);
                    Destroy(gameObject);
                    return;
                }
            }

            if (itemInfo.isResourceOnly && !currentTile.isOccupiedWithBuilding && !currentTile.GetComponent<CraftingManager>().isCrafting) {
                if (currentTile.PlaceResource(itemInfo.onTilePrefab)) {
                    currentTile.UpdateCurrentPlacedResourceList(itemInfo);
                    currentTile.isOccupiedWithResources = true;
                    if (i == 1) {
                        Destroy(gameObject);
                    }
                    currentTile.resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
                    continue;
                } else { 
                    currentTile.resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
                    currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                    DetermineExtraItems(i);
                    Destroy(gameObject);
                    return;
                }
            }

            if (currentTile != null && !currentTile.isOccupiedWithBuilding && itemInfo.checkValidTiles(currentTile.GetComponent<Tile>().tileInfo) && !currentTile.isOccupiedWithResources) {
                GameObject thisItem = Instantiate(itemInfo.onTilePrefab, currentTile.transform.position, transform.rotation);
                thisItem.transform.parent = currentTile.transform;
                currentTile.isOccupiedWithBuilding = true;
                currentTile.UpdateCurrentPlacedItem(itemInfo, thisItem);
                DetermineExtraItems(i - 1);
                currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                Destroy(gameObject);
                return;
            } else {
                if (i - 1 == 0) {
                    DetermineExtraItems(i);
                } else {
                    DetermineExtraItems(i);
                }
                Destroy(gameObject);
                return;
            }
        }

        currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
    }

    private void DetermineExtraItems(int amountExtra) {

        for (int i = 0; i < amountExtra; i++)
        {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
            audioManager.Play("Pop");
        }
    }

}
