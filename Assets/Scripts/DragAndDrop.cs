using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class DragAndDrop : MonoBehaviour
{

    private Vector3 dragOffset;
    private Camera mainCamera;
    public bool isActive = false;
    private DraggableItem draggableItem;
    private SellButton sellButton;
    private EconomyManager economyManager;
    private int tileLayerMask;

    private void Awake() {
        mainCamera = Camera.main;
        draggableItem = GetComponent<DraggableItem>();
        sellButton = FindObjectOfType<SellButton>();
        economyManager = FindObjectOfType<EconomyManager>();
    }

    private void Start() {
        tileLayerMask = LayerMask.GetMask("Tile");
    }


    private void OnMouseDown() {
        dragOffset = transform.position - UtilsClass.GetMouseWorldPosition();
        isActive = true;
    }

    private void OnMouseDrag() {
        transform.position = UtilsClass.GetMouseWorldPosition() + dragOffset;

        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.zero, 100f, tileLayerMask);

        if (hit && isActive)
        {
            draggableItem.setActiveTile(hit.transform.gameObject);
        }
    }

    private void OnMouseUp() {
        draggableItem.PlaceItemOnTile();
        draggableItem.tileHighlightOff();
        isActive = false;

        if (sellButton.overSellBox) {
            economyManager.SellItem(GetComponent<DraggableItem>().itemInfo.coinValue);
            Destroy(gameObject);
        }
    }

    
}
