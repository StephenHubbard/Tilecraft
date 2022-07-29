using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private CinemachineTransposer cinemachineTransposer;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        cinemachineVirtualCamera.m_Lens.OrthographicSize = 5f;

    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.Instance.GetCameraMoveVector();

        float moveSpeed = 10f;

        Vector3 moveVector = transform.up * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }


    private void HandleZoom()
    {
        float zoomIncreaseAmount = 2f;
        float currentZoom = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        float targetZoom = currentZoom;
        targetZoom += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;
        targetZoom = Mathf.Clamp(targetZoom, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float zoomSpeed = 5f;

        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSpeed);
    }

}
