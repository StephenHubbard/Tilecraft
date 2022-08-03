using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class DragAndDrop : MonoBehaviour
{

    public Vector3 dragOffset;
    private Camera mainCamera;
    public bool isActive = false;
    private DraggableItem draggableItem;
    private SellButton sellButton;
    private EconomyManager economyManager;
    private int tileCloudLayerMask;
    private int interactableLayerMask;
    private Stackable stackable;

    private void Awake() {
        stackable = GetComponent<Stackable>();
        mainCamera = Camera.main;
        draggableItem = GetComponent<DraggableItem>();
        sellButton = FindObjectOfType<SellButton>();
        economyManager = FindObjectOfType<EconomyManager>();
        tileCloudLayerMask = LayerMask.GetMask("Tile");
        tileCloudLayerMask += LayerMask.GetMask("Clouds");
        interactableLayerMask = LayerMask.GetMask("Interactable");
    }


    private void Update() {
        if (isActive) {
            OnMouseDragCustom();
        }
    }

    public void OnMouseDownCustom() {
        dragOffset = transform.position - UtilsClass.GetMouseWorldPosition();
        isActive = true;
        stackable.FindAmountOfChildren(transform);
        stackable.DetachFromParent();
    }


    public void OnMouseDragCustom() {
        transform.position = UtilsClass.GetMouseWorldPosition() + dragOffset;

        // tile and cloud detection
        RaycastHit2D[] hit = Physics2D.RaycastAll(gameObject.transform.position, Vector2.zero, 100f, tileCloudLayerMask);
        if (hit.Length > 0) {
            if (hit[0].transform.GetComponent<Tile>() && isActive)
            {
                draggableItem.setActiveTile(hit[0].transform.gameObject);
            } else {
                draggableItem.tileHighlightOff();
            }
        } else {
            draggableItem.CustomerOnTriggerExit2D();
        }

        // stack detection
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, interactableLayerMask);
        if (hit2.Length > 1) {
            if (hit2[1].transform.gameObject.GetComponent<Stackable>() && hit2[1].transform.gameObject.GetComponent<DraggableItem>().itemInfo == stackable.itemInfo) {
                if (hit2[1].transform.root != transform) {
                    stackable.potentialParentItem = hit2[1].transform.root;
                }
            }
        } else {
            stackable.potentialParentItem = null;
        }
    }

    public void OnMouseUpCustom() {
        if (draggableItem.currentTile) {
            draggableItem.PlaceItemOnTile(stackable.amountOfChildItems);
        }

        if (sellButton.overSellBox) {
            economyManager.SellItem(GetComponent<DraggableItem>().itemInfo.coinValue, stackable.amountOfChildItems);
            Destroy(gameObject);
        }

        if (stackable.potentialParentItem) {
            stackable.AttachToParent();
        } 
        
        isActive = false;

    }

    
}
