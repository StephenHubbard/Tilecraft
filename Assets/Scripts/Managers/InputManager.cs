using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private int interactableLayerMask;

    private DragAndDrop activeObject;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        interactableLayerMask = LayerMask.GetMask("Interactable");

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

        if (Input.GetMouseButtonDown(0)) {
            Transform lowestZGameObject = null;

            RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, interactableLayerMask);

            if (hit.Length > 0) {
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
        }

        if (activeObject) {
            if (Input.GetMouseButtonUp(0)) {
                activeObject.OnMouseUpCustom();
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
    }
}
