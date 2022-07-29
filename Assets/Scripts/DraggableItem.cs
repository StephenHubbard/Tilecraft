using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    [SerializeField] private ItemInfo itemInfo;
    
    private GameObject tileHighlight;
    private Tile currentTile;

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

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.GetComponent<Tile>() && GetComponent<DragAndDrop>().isActive) {
            currentTile = other.gameObject.GetComponent<Tile>();
        } 
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
        if (currentTile != null && !currentTile.isOccupied && itemInfo.tileInfo == currentTile.GetComponent<Tile>().tileInfo) {
            GameObject thisItem = Instantiate(itemInfo.onTilePrefab, currentTile.transform.position, transform.rotation);
            thisItem.transform.parent = currentTile.transform;
            currentTile.isOccupied = true;
            Destroy(gameObject);
        }
    }

}
