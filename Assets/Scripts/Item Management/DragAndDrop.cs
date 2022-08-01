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
    private HighlightedBorder highlightedBorder;
    private Stackable stackable;

    private void Awake() {
        stackable = GetComponent<Stackable>();
        mainCamera = Camera.main;
        highlightedBorder = FindObjectOfType<HighlightedBorder>();
        draggableItem = GetComponent<DraggableItem>();
        sellButton = FindObjectOfType<SellButton>();
        economyManager = FindObjectOfType<EconomyManager>();
    }

    private void Start() {
        tileLayerMask = LayerMask.GetMask("Tile");
    }


    private void OnMouseDown() {
        dragOffset = transform.position - UtilsClass.GetMouseWorldPosition();
        highlightedBorder.UpdateCurrentItemInHand(gameObject.GetComponent<DraggableItem>().itemInfo);
        isActive = true;
        stackable.isInStackAlready = false;
        stackable.FindAmountOfChildren(transform);
    }

    private void OnMouseDrag() {
        transform.position = UtilsClass.GetMouseWorldPosition() + dragOffset;

        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.zero, 100f, tileLayerMask);

        if (hit && isActive)
        {
            draggableItem.setActiveTile(hit.transform.gameObject);
        } else {
            draggableItem.tileHighlightOff();
        }
    }

    private void OnMouseUp() {
        if (draggableItem.currentTile) {
            draggableItem.PlaceItemOnTile(stackable.amountOfChildItems);
        }
        

        if (sellButton.overSellBox) {
            economyManager.SellItem(GetComponent<DraggableItem>().itemInfo.coinValue, stackable.amountOfChildItems);
            Destroy(gameObject);
        }

        if (stackable.potentialParentItem) {
            stackable.AttachToParent();
        } else {
            stackable.DetachFromParent();
        }

        isActive = false;
        FindObjectOfType<HighlightedBorder>().currentHeldItem = null;
    }

    
}
