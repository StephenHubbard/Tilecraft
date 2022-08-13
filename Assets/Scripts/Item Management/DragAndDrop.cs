using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

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
    private Worker potentialWorkerToFeed;

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
            foreach (var item in hit2)
            {
                if (item.transform.GetComponent<Worker>()) {
                    potentialWorkerToFeed = item.transform.GetComponent<Worker>();
                    InputManager.instance.CircleHighlightOn();
                    InputManager.instance.circleHighlight.transform.position = potentialWorkerToFeed.transform.position;
                    return;
                }
            }
        } else {
            potentialWorkerToFeed = null;
            InputManager.instance.CircleHighlightOff();
        }
    }

    public void OnMouseUpCustom() {
        isActive = false;
        stackable.FindAmountOfChildren(transform);


        if (potentialWorkerToFeed) {
            potentialWorkerToFeed.FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
            InputManager.instance.CircleHighlightOff();
            Destroy(gameObject);
            return;
        }

        if (stackable.potentialParentItem) {
            stackable.AttachToParent(true);
            return;
        } 

        if (draggableItem.currentTile) {
            draggableItem.PlaceItemOnTile(stackable.amountOfChildItems);
            return;
        }
    }
}
