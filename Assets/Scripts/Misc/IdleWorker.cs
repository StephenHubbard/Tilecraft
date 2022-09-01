using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleWorker : MonoBehaviour
{
    [SerializeField] private Transform cameraController;
    [SerializeField] private GameObject circleHighlight;

    private void Awake() {
        cameraController = FindObjectOfType<CameraController>().transform;
    }

    public void FindIdleWorkerButton() {
        Worker[] allWorkers = FindObjectsOfType<Worker>();

        foreach (var worker in allWorkers)
        {
            if (worker.GetComponent<PlacedItem>() && worker.myAnimator.GetBool("isWorking") == false && worker.isBabyMaking == false) {
                cameraController.transform.position = worker.transform.position + new Vector3(0, -2f, 0);
                GameObject newCircle = Instantiate(circleHighlight, worker.transform.position, transform.rotation);
                StartCoroutine(destroyCircle(newCircle));
                return;
            }

            if (!worker.GetComponent<PlacedItem>()) {
                cameraController.transform.position = worker.transform.position + new Vector3(0, -2f, 0);
                GameObject newCircle = Instantiate(circleHighlight, worker.transform.position + new Vector3(0, 0.144f, 0), transform.rotation);
                StartCoroutine(destroyCircle(newCircle));
                return;
            }
        }
    }

    private IEnumerator destroyCircle(GameObject newCircle) {
        yield return new WaitForSeconds(.9f);
        Destroy(newCircle);
    }
}
