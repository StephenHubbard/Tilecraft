using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [SerializeField] public ItemInfo itemInfo;
    
    private GameObject tileHighlight;
    public Tile currentTile;

    private void Awake() {
        tileHighlight = GameObject.Find("Highlighted Border");
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

    public void PlaceItemOnTile() {
        if (itemInfo.name == "Worker") {
            if (currentTile.PlaceWorker(itemInfo.onTilePrefab)) {
                currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                currentTile.GetComponent<CraftingManager>().IncreaseWorkerCount();
                Destroy(gameObject);
                return;
            } else { 
                print("no worker spots available");
                return;
            }
        }

        if (itemInfo.isResourceOnly && !currentTile.isOccupiedWithBuilding && !currentTile.GetComponent<CraftingManager>().isCrafting) {
            if (currentTile.PlaceResource(itemInfo.onTilePrefab)) {
                currentTile.UpdateCurrentPlacedResourceList(itemInfo);
                currentTile.isOccupiedWithResources = true;
                currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                Destroy(gameObject);
                return;
            } else { 
                print("no resource spots available");
                return;
            }
        }

        if (currentTile != null && !currentTile.isOccupiedWithBuilding && itemInfo.checkValidTiles(currentTile.GetComponent<Tile>().tileInfo) && !currentTile.isOccupiedWithResources) {
            GameObject thisItem = Instantiate(itemInfo.onTilePrefab, currentTile.transform.position, transform.rotation);
            thisItem.transform.parent = currentTile.transform;
            currentTile.isOccupiedWithBuilding = true;
            currentTile.UpdateCurrentPlacedItem(itemInfo, thisItem);
            currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
            Destroy(gameObject);
            return;
        }

    }

}
