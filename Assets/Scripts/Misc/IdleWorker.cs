using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWorker : MonoBehaviour
{
    [SerializeField] private Transform cameraController;

    private void Awake() {
        cameraController = FindObjectOfType<CameraController>().transform;
    }

    public void FindIdleWorkerButton() {
        Worker[] allWorkers = FindObjectsOfType<Worker>();

        foreach (var worker in allWorkers)
        {
            if (worker.GetComponent<PlacedItem>() && worker.myAnimator.GetBool("isWorking") == false) {
                cameraController.transform.position = worker.transform.position + new Vector3(0, -2f, 0);
                return;
            }
        }
    }
}
