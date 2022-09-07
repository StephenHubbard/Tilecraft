using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour
{
    [SerializeField] public GameObject tileHighlight;

    public static InputManager instance { get; private set; }

    [SerializeField] private LayerMask interactableLayerMask = new LayerMask();
    [SerializeField] private LayerMask tileDetectionLayerMask = new LayerMask();

    [SerializeField] private CursorManager.CursorType cursorTypeArrow;
    [SerializeField] private CursorManager.CursorType cursorOpenHand;
    [SerializeField] private CursorManager.CursorType cursorClosedHand;

    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private GameObject pauseMenu;

    private DragAndDropCustom activeObject;

    public bool isOnMainMenu = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + instance);
            Destroy(gameObject);
            return;
        }
        instance = this;

    }

        public Vector2 GetMouseScreenPosition()
    {
        return Input.mousePosition;
    }

    

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Tab)) {
            Encyclopedia.instance.OpenEncylopedia();
            // ToolTipManager.instance.isOverUI = false;
        }

        // for debugging disable before uploading build
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            FindObjectOfType<IdleWorker>().FindIdleWorkerButton();
        }

        UpdateTileInput();
        UpdateInteractablesInput();
        UpdateEncyclopediaScroll();
    }

    private void UpdateEncyclopediaScroll()
    {
        if (ToolTipManager.instance.isOverUI == false) { return; }

        if (Input.mouseScrollDelta.y > 0)
            {
                scrollbar.value += .3f;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                scrollbar.value -= .3f;
            }
    }

    private void UpdateTileInput() {
        if (Menu.instance.isPaused) { return; }

        RaycastHit2D[] hitArray = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, tileDetectionLayerMask);

        Tile thisTile = null;

        bool isRayBeingBlocked = false;

        if (hitArray.Length > 0) {

            foreach (var item in hitArray)
            {
                thisTile = item.transform.GetComponent<Tile>();
            }

            if (thisTile != null) {
                foreach (var item in hitArray)
                {
                    if (item.transform.GetComponent<Stackable>() || item.transform.GetComponent<Cloud>()) { 
                        isRayBeingBlocked = true;

                        if (item.transform.GetComponent<Stackable>()) {
                            if (item.transform.GetComponent<DragAndDropCustom>().isActive) {
                                isRayBeingBlocked = false;
                            }
                        }
                    } 

                }

                if (thisTile.currentPlacedItem && thisTile.currentPlacedItem.GetComponent<Garbage>() && activeObject && (activeObject.GetComponent<DraggableItem>().itemInfo.isResourceOnly || activeObject.GetComponent<DraggableItem>().itemInfo.isStationary)) {
                    thisTile.currentPlacedItem.GetComponent<Garbage>().GarbageSpriteOpen();
                } else {
                    foreach (var garbage in FindObjectsOfType<Garbage>())
                    {
                        garbage.GarbageSpriteClosed();
                    }
                }


                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1)) { 
                    if (isRayBeingBlocked == false) {
                        thisTile.GetComponent<Tile>().PluckItemsOffTileAll();
                    }
                }

                if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(1)) {
                    if (isRayBeingBlocked == false) {
                        thisTile.GetComponent<Tile>().PluckItemsOffTile();
                    }
                }

            }

            if (isRayBeingBlocked == false && thisTile != null) {
                tileHighlight.GetComponent<SpriteRenderer>().enabled = true;

                tileHighlight.transform.position = thisTile.transform.position;
            } else {
                tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
            }
        } else {
            foreach (var garbage in FindObjectsOfType<Garbage>())
            {
                garbage.GarbageSpriteClosed();
            }
        }

    }

    private void UpdateInteractablesInput() {
            if (Menu.instance.isPaused) { return; }

            Transform lowestZGameObject = null;

            RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, interactableLayerMask);

            if (hit.Length > 0) {
                CursorManager.instance.SetActiveCursorType(cursorOpenHand);

                if (Input.GetMouseButtonDown(0)) {
                    foreach (var item in hit)
                    {
                        if (lowestZGameObject == null) {
                            lowestZGameObject = item.transform;
                        } else if (item.transform.position.y < lowestZGameObject.position.y) {
                            lowestZGameObject = item.transform;
                        }
                    }
                    activeObject = lowestZGameObject.gameObject.GetComponent<DragAndDropCustom>();
                    activeObject.OnMouseDownCustom();
                } 
            } else {
                CursorManager.instance.SetActiveCursorType(cursorTypeArrow);
            }

        if (activeObject) {
            CursorManager.instance.SetActiveCursorType(cursorClosedHand);

            if (Input.GetMouseButtonUp(0)) {
                activeObject.OnMouseUpCustom();
                activeObject = null;
                CursorManager.instance.SetActiveCursorType(cursorOpenHand);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0) && TutorialManager.instance.storageActive) {
            Transform lowestZGameObject2 = null;

            RaycastHit2D[] hit2 = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, interactableLayerMask);

            if (hit.Length > 0) {
                foreach (var item in hit)
                {
                    if (lowestZGameObject2 == null) {
                        lowestZGameObject2 = item.transform;
                    } else if (item.transform.position.y < lowestZGameObject2.position.y) {
                        lowestZGameObject2 = item.transform;
                    }
                }

                if (StorageContainer.instance.CheckIfStorageHasSpace(lowestZGameObject2.GetComponent<Stackable>().itemInfo, lowestZGameObject2.gameObject)) {
                    lowestZGameObject2.GetComponent<Stackable>().FindAmountOfChildren(lowestZGameObject2);
                    StorageContainer.instance.AddToStorage(lowestZGameObject2.GetComponent<Stackable>().itemInfo, lowestZGameObject2.GetComponent<Stackable>().amountOfChildItems);
                    Destroy(lowestZGameObject.gameObject);
                }
            }
        }
    }

    public Vector2 GetCameraMoveVector()
    {
        Vector2 inputMoveDir = new Vector2(0, 0);
        
        if (!isOnMainMenu) { 
            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDir.y = +1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                inputMoveDir.y = -1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                inputMoveDir.x = -1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                inputMoveDir.x = +1f;
            }
        }

        return inputMoveDir;
    }

    public float GetCameraZoomAmount()
    {
        if (!isOnMainMenu) { 
            if (!ToolTipManager.instance.isOverUI) { 
                float zoomAmount = 0f;

                if (Input.mouseScrollDelta.y > 0)
                {
                    zoomAmount = -1f;
                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    zoomAmount = +1f;
                }

                return zoomAmount;
            } else {
                return 0;
            }
        }

        return 0;
    }

}
