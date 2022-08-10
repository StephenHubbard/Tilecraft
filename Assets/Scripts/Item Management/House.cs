using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class House : MonoBehaviour
{
    [SerializeField] private GameObject sliderCanvas;
    [SerializeField] private Slider tileSlider;
    [SerializeField] private float timeToHaveBabyCurrent;
    [SerializeField] private float timeToHaveBabyTotal = 10f;
    [SerializeField] private Transform[] workerPoints;
    [SerializeField] private GameObject workerItemPrefab;

    public bool isBabyMaking = false;

    private void Start() {
        HousingManager.instance.DetectHowManyHouses();
        workerPoints = GetComponentInParent<Tile>().workerPoints;
        timeToHaveBabyCurrent = timeToHaveBabyTotal;
    }

    private void Update() {
        if (isBabyMaking) {
            tileSlider.value = timeToHaveBabyCurrent;
            timeToHaveBabyCurrent -= Time.deltaTime;
            if (timeToHaveBabyCurrent <= 0f) {
                StopBabyMaking();
            }
        }
    }

    public void DetectBabyMaking() {
        int amountOfCurrentWorkersOnTile = 0;

        foreach (var workerPoint in workerPoints)
        {
            if (workerPoint.childCount == 1) {
                amountOfCurrentWorkersOnTile++;
            }
        }

        if (amountOfCurrentWorkersOnTile == 2) {
            if (HousingManager.instance.currentWorkers < HousingManager.instance.maximumWorkers) {
                StartBabyMaking();
            } else {
                print("not enough housing");
            }
        }
    }

    private void StartBabyMaking() {
        sliderCanvas.SetActive(true);
        tileSlider.maxValue = timeToHaveBabyCurrent;
        isBabyMaking = true;
    }

    public void StopBabyMaking() {
        sliderCanvas.SetActive(false);
        isBabyMaking = false;

        if (timeToHaveBabyCurrent <= .1f) {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(workerItemPrefab, spawnItemsVector3, transform.rotation);
        }

        timeToHaveBabyCurrent = timeToHaveBabyTotal;
    }
}
