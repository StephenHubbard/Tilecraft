using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class InputManager : MonoBehaviour
{
    [SerializeField] public GameObject circleHighlight;
    [SerializeField] public GameObject tileHighlight;

    public static InputManager instance { get; private set; }

    [SerializeField] private LayerMask interactableLayerMask = new LayerMask();
    [SerializeField] private LayerMask tileDetectionLayerMask = new LayerMask();

    [SerializeField] private CursorManager.CursorType cursorTypeArrow;
    [SerializeField] private CursorManager.CursorType cursorOpenHand;
    [SerializeField] private CursorManager.CursorType cursorClosedHand;

    private DragAndDrop activeObject;

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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            Encyclopedia.instance.OpenEncylopedia();
            ToolTipManager.instance.isOverUI = false;
        }

        UpdateTileInput();
        UpdateInteractablesInput();
    }

    private void UpdateTileInput() {
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
                    } 
                }

                if (Input.GetMouseButtonDown(1)) {
                    if (isRayBeingBlocked == false) {
                        thisTile.GetComponent<Tile>().PluckItemsOffTile();
                    }
                }

                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift) && thisTile.currentPlacedItem && thisTile.currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary && thisTile.currentPlacedItem.GetComponent<PlacedItem>().itemInfo.potentialOffSpring.Length > 0) {
                    if (isRayBeingBlocked == false) {
                        thisTile.GetComponent<Tile>().ToggleAutoSell();
                    }
                }
            }

            if (isRayBeingBlocked == false && thisTile != null) {
                tileHighlight.GetComponent<SpriteRenderer>().enabled = true;

                tileHighlight.transform.position = thisTile.transform.position;
            } else {
                tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void UpdateInteractablesInput() {
            Transform lowestZGameObject = null;

            RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, interactableLayerMask);

            if (hit.Length > 0) {
                CursorManager.Instance.SetActiveCursorType(cursorOpenHand);

                if (Input.GetMouseButtonDown(0)) {
                    foreach (var item in hit)
                    {
                        if (lowestZGameObject == null) {
                            lowestZGameObject = item.transform;
                        } else if (item.transform.position.y < lowestZGameObject.position.y) {
                            lowestZGameObject = item.transform;
                        }
                    }
                    activeObject = lowestZGameObject.gameObject.GetComponent<DragAndDrop>();
                    activeObject.OnMouseDownCustom();
                } 
            } else {
                CursorManager.Instance.SetActiveCursorType(cursorTypeArrow);
            }

        if (activeObject) {
            CursorManager.Instance.SetActiveCursorType(cursorClosedHand);

            if (Input.GetMouseButtonUp(0)) {
                activeObject.OnMouseUpCustom();
                activeObject = null;
                CursorManager.Instance.SetActiveCursorType(cursorOpenHand);
            }
        }

        if (Input.GetMouseButtonDown(1)) {
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
                lowestZGameObject2.GetComponent<Stackable>().FindAmountOfChildren(lowestZGameObject2);
                EconomyManager.instance.SellItem(lowestZGameObject2.gameObject, lowestZGameObject2.GetComponent<DraggableItem>().itemInfo.coinValue, lowestZGameObject2.GetComponent<Stackable>().amountOfChildItems);

                if (lowestZGameObject2.gameObject.GetComponent<Worker>()) {
                    HousingManager.instance.DetectTotalPopulation();
                    HousingManager.instance.AllHousesDetectBabyMaking();
                }

                if (lowestZGameObject2.gameObject.GetComponent<Archer>()) {
                    HousingManager.instance.DetectTotalPopulation();
                    HousingManager.instance.AllHousesDetectBabyMaking();
                }

                if (lowestZGameObject2.gameObject.GetComponent<Knight>()) {
                    HousingManager.instance.DetectTotalPopulation();
                    HousingManager.instance.AllHousesDetectBabyMaking();
                }
            }
        }

    }

    public Vector2 GetCameraMoveVector()
    {
        Vector2 inputMoveDir = new Vector2(0, 0);

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

        return inputMoveDir;
    }

    public float GetCameraZoomAmount()
    {
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

    public void CircleHighlightOn() {
        circleHighlight.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void CircleHighlightOff() {
        circleHighlight.GetComponent<SpriteRenderer>().enabled = false;
    }
}
