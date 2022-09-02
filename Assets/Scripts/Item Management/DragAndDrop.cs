using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Rendering;

public class DragAndDrop : MonoBehaviour
{

    public Vector3 dragOffset;
    public bool isActive = false;
    private DraggableItem draggableItem;
    private SellButton sellButton;
    private EconomyManager economyManager;
    private int tileCloudLayerMask;
    private int interactableLayerMask;
    private Stackable stackable;
    private GameObject potentialToFeed;
    private GameObject potentialWorkerToEquip;

    private void Awake() {
        stackable = GetComponent<Stackable>();
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
        OnMouseDragCustom();
    }

    private void HandleTileAndClouds() {
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
            foreach (var item in hit2) 
            {
                if (item.transform.gameObject.GetComponent<Stackable>() && item.transform.gameObject.GetComponent<DraggableItem>().itemInfo == stackable.itemInfo && item.transform.root != transform) 
                {
                    stackable.potentialParentItem = item.transform.root;
                    return;
                }
            }
        } else {
            stackable.potentialParentItem = null;
        }
    }

    private void HandleOverStorageDetection() {
        DragAndDrop[] allChildItems = GetComponentsInChildren<DragAndDrop>();

        if (StorageContainer.instance.isOverStorage) {
            StorageContainer.instance.ActivateWhiteBorderOn();

            foreach (var item in allChildItems)
            {
                item.transform.GetChild(0).GetComponent<SortingGroup>().sortingOrder = 2;
            }
        } else {
            StorageContainer.instance.ActivateWhiteBorderOff();

            foreach (var item in allChildItems)
            {
                item.transform.GetChild(0).GetComponent<SortingGroup>().sortingOrder = 0;
            }
        }
    }


    public void OnMouseDragCustom() {
        transform.position = UtilsClass.GetMouseWorldPosition() + dragOffset;

        HandleOverStorageDetection();

        HandleTileAndClouds();

        
    }

    public void OnMouseUpCustom() {
        isActive = false;
        stackable.FindAmountOfChildren(transform);

        StorageContainer.instance.ActivateWhiteBorderOff();

        if (StorageContainer.instance.isOverStorage && StorageContainer.instance.CheckIfStorageHasSpace(GetComponent<DraggableItem>().itemInfo)) {
            StorageContainer.instance.AddToStorage(draggableItem.itemInfo, stackable.amountOfChildItems);
            Destroy(gameObject);
            return;
        } else if (StorageContainer.instance.isOverStorage) {
            transform.position = transform.position + new Vector3(0, -1f, 0);
            AudioManager.instance.Play("Pop");
        }

        if (stackable.potentialParentItem) {
            stackable.AttachToParent(true);
            return;
        } 

        if (draggableItem.currentTile) {
            draggableItem.PlaceItemOnTile(stackable.amountOfChildItems);
            return;
        }

        if (stackable.amountOfChildItems == 1) {
            GetComponent<Stackable>().FindNearbySameQOL(true);
        }
    }
}
