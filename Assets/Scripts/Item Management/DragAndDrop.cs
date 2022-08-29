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
                    potentialToFeed = item.transform.GetComponent<Worker>().gameObject;
                    InputManager.instance.CircleHighlightOn();
                    InputManager.instance.circleHighlight.transform.position = potentialToFeed.transform.position;
                    return;
                }

                if (item.transform.GetComponent<Archer>()) {
                    potentialToFeed = item.transform.GetComponent<Archer>().gameObject;
                    InputManager.instance.CircleHighlightOn();
                    InputManager.instance.circleHighlight.transform.position = potentialToFeed.transform.position;
                    return;
                }

                if (item.transform.GetComponent<Knight>()) {
                    potentialToFeed = item.transform.GetComponent<Knight>().gameObject;
                    InputManager.instance.CircleHighlightOn();
                    InputManager.instance.circleHighlight.transform.position = potentialToFeed.transform.position;
                    return;
                }
            }
        } else {
            potentialToFeed = null;
            InputManager.instance.CircleHighlightOff();
        }

        // equipment
        if (hit2.Length > 1 && GetComponent<Weapon>()) {
            foreach (var item in hit2)
            {
                if (item.transform.GetComponent<Worker>() || item.transform.GetComponent<Knight>() || item.transform.GetComponent<Archer>()) {
                    if (item.transform.GetComponent<Worker>()) {
                        potentialWorkerToEquip = item.transform.GetComponent<Worker>().gameObject;
                    }

                    if (item.transform.GetComponent<Knight>()) {
                        potentialWorkerToEquip = item.transform.GetComponent<Knight>().gameObject;
                    }

                    if (item.transform.GetComponent<Archer>()) {
                        potentialWorkerToEquip = item.transform.GetComponent<Archer>().gameObject;
                    }
                    InputManager.instance.CircleHighlightOn();
                    InputManager.instance.circleHighlight.transform.position = potentialWorkerToEquip.transform.position;
                    return;
                }
            }
        } else {
            potentialWorkerToEquip = null;
            InputManager.instance.CircleHighlightOff();
        }
    }

    public void OnMouseUpCustom() {
        isActive = false;
        stackable.FindAmountOfChildren(transform);


        if (potentialToFeed) {
            if (potentialToFeed.GetComponent<Worker>()) {
                potentialToFeed.GetComponent<Worker>().FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
                InputManager.instance.CircleHighlightOff();
                Destroy(gameObject);
                return;
            }

            if (potentialToFeed.GetComponent<Knight>()) {
                potentialToFeed.GetComponent<Knight>().FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
                InputManager.instance.CircleHighlightOff();
                Destroy(gameObject);
                return;
            }
            if (potentialToFeed.GetComponent<Archer>()) {
                potentialToFeed.GetComponent<Archer>().FeedWorker(GetComponent<DraggableItem>().itemInfo.foodValue * stackable.amountOfChildItems, true);
                InputManager.instance.CircleHighlightOff();
                Destroy(gameObject);
                return;
            }
        }

        if (potentialWorkerToEquip) {

            if (potentialWorkerToEquip.transform.GetComponent<Worker>()) {
                potentialWorkerToEquip.GetComponent<Worker>().EquipWorker(GetComponent<Weapon>());
            }

            if (potentialWorkerToEquip.transform.GetComponent<Knight>()) {
                potentialWorkerToEquip.GetComponent<Knight>().EquipWorker(GetComponent<Weapon>());
            }

            if (potentialWorkerToEquip.transform.GetComponent<Archer>()) {
                potentialWorkerToEquip.GetComponent<Archer>().EquipWorker(GetComponent<Weapon>());
            }

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

        if (stackable.amountOfChildItems == 1) {
            GetComponent<Stackable>().FindNearbySameQOL(true);
        }
    }
}
