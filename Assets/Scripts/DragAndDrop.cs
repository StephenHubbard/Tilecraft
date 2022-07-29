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

    private void Awake() {
        mainCamera = Camera.main;
        draggableItem = GetComponent<DraggableItem>();
        sellButton = FindObjectOfType<SellButton>();
        economyManager = FindObjectOfType<EconomyManager>();
    }


    private void OnMouseDown() {
        dragOffset = transform.position - UtilsClass.GetMouseWorldPosition();
        isActive = true;
    }

    private void OnMouseDrag() {
        transform.position = UtilsClass.GetMouseWorldPosition() + dragOffset;
    }

    private void OnMouseUp() {
        draggableItem.PlaceItemOnTile();
        draggableItem.tileHighlightOff();
        isActive = false;

        if (sellButton.overSellBox) {
            economyManager.SellItem(1);
            Destroy(gameObject);
        }
    }

    
}
