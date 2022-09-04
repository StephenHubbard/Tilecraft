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
    private int tileCloudLayerMask;
    private int interactableLayerMask;
    private Stackable stackable;
    private GameObject potentialToFeed;
    private GameObject potentialFoodItem;
    private GameObject potentialWorkerToEquip;

    private void Awake() {
        stackable = GetComponent<Stackable>();
        draggableItem = GetComponent<DraggableItem>();
        sellButton = FindObjectOfType<SellButton>();
        tileCloudLayerMask = LayerMask.GetMask("Tile");
        tileCloudLayerMask += LayerMask.GetMask("Clouds");
        interactableLayerMask = LayerMask.GetMask("Interactable");
    }


    private void Update() {
        if (isActive) {
            OnMouseDragCustom();

            if (potentialToFeed) {
                ToolTipManager.instance.DetectFeedPopTooltip(potentialToFeed);
            } 
        }

    }

    public void OnMouseDownCustom() {
        dragOffset = transform.position - UtilsClass.GetMouseWorldPosition();
        isActive = true;
        stackable.FindAmountOfChildren(transform);
        stackable.DetachFromParent();
        OnMouseDragCustom();
        GetComponent<DraggableItem>().CanPlaceOnTile = false;
        StartCoroutine(GetComponent<DraggableItem>().CanPlaceItemOnTileDelay());
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

        // feeding workers
        if (hit2.Length > 1 && GetComponent<Food>()) {
            
            Population[] allPop = FindObjectsOfType<Population>();

            foreach (Population pop in allPop)
            {
                pop.ToggleCircleHighlightOff();
            }

            foreach (var item in hit2)
            {
                if (item.transform.GetComponent<Population>() && !item.transform.GetComponent<Population>().isMaxLevel) {
                    if (item.transform.GetComponent<Worker>()) {
                        potentialToFeed = item.transform.GetComponent<Worker>().gameObject;
                        item.transform.GetComponent<Population>().ToggleCircleHighlightOn();
                        return;
                    }

                    if (item.transform.GetComponent<Archer>()) {
                        potentialToFeed = item.transform.GetComponent<Archer>().gameObject;
                        item.transform.GetComponent<Population>().ToggleCircleHighlightOn();
                        return;
                    }

                    if (item.transform.GetComponent<Knight>()) {
                        potentialToFeed = item.transform.GetComponent<Knight>().gameObject;
                        item.transform.GetComponent<Population>().ToggleCircleHighlightOn();
                        return;
                    }
                }
            }
        } else {
            if (potentialToFeed) {
                Population[] allPop = FindObjectsOfType<Population>();

                foreach (Population pop in allPop)
                {
                    pop.ToggleCircleHighlightOff();
                }

                potentialToFeed = null;
            }
        }

        if (hit2.Length > 1 && GetComponent<Population>()) {
            foreach (var item in hit2)
            {
                if (GetComponent<Population>() && !GetComponent<Population>().isMaxLevel) { 
                    if (item.transform.GetComponent<Food>()) {
                        potentialToFeed = this.gameObject;
                        potentialFoodItem = item.transform.gameObject;
                        GetComponent<Population>().ToggleCircleHighlightOn();
                        return;
                    }
                }
            }
        } else {
            if (potentialToFeed) {
                potentialToFeed.transform.GetComponent<Population>().ToggleCircleHighlightOff();
                potentialToFeed = null;
                potentialFoodItem = null;
            }
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

        if (potentialToFeed) {
            if (potentialToFeed.GetComponent<Worker>()) {
                potentialToFeed.transform.GetComponent<Population>().ToggleCircleHighlightOff();
                if (potentialFoodItem) {
                    potentialToFeed.GetComponent<Worker>().FeedWorker(potentialFoodItem.GetComponent<DraggableItem>().itemInfo.foodValue * potentialFoodItem.GetComponent<Stackable>().amountOfChildItems, true);
                    Destroy(potentialFoodItem);
                } else {
                    potentialToFeed.GetComponent<Worker>().FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
                    Destroy(gameObject);
                }
                return;
            }

            if (potentialToFeed.GetComponent<Knight>()) {
                potentialToFeed.transform.GetComponent<Population>().ToggleCircleHighlightOff();
                if (potentialFoodItem) {
                    potentialToFeed.GetComponent<Knight>().FeedWorker(potentialFoodItem.GetComponent<DraggableItem>().itemInfo.foodValue * potentialFoodItem.GetComponent<Stackable>().amountOfChildItems, true);
                    Destroy(potentialFoodItem);
                } else {
                    potentialToFeed.GetComponent<Knight>().FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
                    Destroy(gameObject);
                }
                return;
            }
            if (potentialToFeed.GetComponent<Archer>()) {
                potentialToFeed.transform.GetComponent<Population>().ToggleCircleHighlightOff();
                if (potentialFoodItem) {
                    potentialToFeed.GetComponent<Archer>().FeedWorker(potentialFoodItem.GetComponent<DraggableItem>().itemInfo.foodValue * potentialFoodItem.GetComponent<Stackable>().amountOfChildItems, true);
                    Destroy(potentialFoodItem);
                } else {
                    potentialToFeed.GetComponent<Archer>().FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
                    Destroy(gameObject);
                }
                return;
            }
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
