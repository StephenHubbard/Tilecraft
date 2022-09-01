using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDragMovement : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private Vector3 ResetCamera;
    private CameraController cameraController;

    private bool drag = false;

    private void Awake() {
        cameraController = FindObjectOfType<CameraController>();
    }

    private void Start()
    {
        ResetCamera = Camera.main.transform.position;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            CursorManager.instance.CursorTypeDragMove();

            Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;

            if(drag == false)
            {
                drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, -2f, 0);
            }

        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            cameraController.transform.position = Origin - Difference;
        }

    }
}
