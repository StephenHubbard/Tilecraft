using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public TileInfo tileInfo;
    [SerializeField] public ItemInfo itemInfo;
    [SerializeField] private GameObject currentPlaceItem;

    private GameObject tileHighlight;

    public bool isOccupied = false;

    private void Awake() {
        tileHighlight = GameObject.Find("Highlighted Border");
        
    }

    public void UpdateCurrentPlaceItem(ItemInfo itemInfo, GameObject thisPlacedItem) {
        this.itemInfo = itemInfo;
        currentPlaceItem = thisPlacedItem;
    }

    private void OnMouseOver() {
        if (isOccupied && Input.GetMouseButtonDown(1)) {
            PluckItemOffTile();
        }

    }


    private void PluckItemOffTile() {
        Destroy(currentPlaceItem);
        Instantiate(itemInfo.draggableItemPrefab, transform.position, transform.rotation);
        isOccupied = false;
    }
}
