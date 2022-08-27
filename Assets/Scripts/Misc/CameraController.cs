using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    private const float MIN_FOLLOW_Z_OFFSET = 4f;
    private const float MAX_FOLLOW_Z_OFFSET = 14f;

    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private Collider2D cameraConfiner;
    [SerializeField] private float cameraBorderBuffer = 7f;
    [SerializeField] private Camera mainCam;

    private CinemachineTransposer cinemachineTransposer;

    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private void Start()
    {
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();

        cinemachineVirtualCamera.m_Lens.OrthographicSize = 6f;

        bottomLeftLimit = cameraConfiner.bounds.min;
        topRightLimit = cameraConfiner.bounds.max;


    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 inputMoveDir = InputManager.instance.GetCameraMoveVector();

        float moveSpeed = 10f;

        Vector3 moveVector = transform.up * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;

        // confiner getting in the way of interactable raycasts event system
        transform.position = (new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x + cameraBorderBuffer, topRightLimit.x - cameraBorderBuffer), Mathf.Clamp(transform.position.y, bottomLeftLimit.y + cameraBorderBuffer, topRightLimit.y - cameraBorderBuffer), transform.position.z));

    }


    private void HandleZoom()
    {
        float zoomIncreaseAmount = 3f;
        float currentZoom = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        float targetZoom = currentZoom;
        targetZoom += InputManager.instance.GetCameraZoomAmount() * zoomIncreaseAmount;
        targetZoom = Mathf.Clamp(targetZoom, MIN_FOLLOW_Z_OFFSET, MAX_FOLLOW_Z_OFFSET);

        float zoomSpeed = 5f;

        cameraBorderBuffer = FindObjectOfType<CinemachineVirtualCamera>().m_Lens.OrthographicSize * 1.65f;

        cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSpeed);
    }

}
