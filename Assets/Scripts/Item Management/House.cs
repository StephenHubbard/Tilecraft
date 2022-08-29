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
        HousingManager.instance.DetectTotalPopulation();
        workerPoints = GetComponentInParent<Tile>().workerPoints;
        timeToHaveBabyCurrent = timeToHaveBabyTotal;
        DetectBabyMaking();

        CompleteHouseTutorial();
    }

    private void CompleteHouseTutorial() {
        if (TutorialManager.instance.tutorialIndexNum == 3) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void CompleteFirstBabyTutorial() {
        if (TutorialManager.instance.tutorialIndexNum == 4) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void Update() {
        if (isBabyMaking) {
            tileSlider.value = timeToHaveBabyCurrent;
            timeToHaveBabyCurrent -= Time.deltaTime;
            if (timeToHaveBabyCurrent <= 0f) {
                StopBabyMaking(true);
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
            if (HousingManager.instance.currentPopulation < HousingManager.instance.maximumPopulation) {
                StartBabyMaking();
            } else {
                HousingManager.instance.BlinkNotEnoughWorkersAnim();
            }
        }
    }

    private void StartBabyMaking() {
        sliderCanvas.SetActive(true);
        tileSlider.maxValue = timeToHaveBabyCurrent;
        isBabyMaking = true;
        foreach (var workerPoint in workerPoints)
        {
            if (workerPoint.childCount > 0) {
                workerPoint.GetChild(0).GetComponent<Worker>().isBabyMaking = true;
            }
        }
    }

    public void StopBabyMaking(bool detectBabyMaking) {
        sliderCanvas.SetActive(false);
        isBabyMaking = false;

        foreach (var workerPoint in workerPoints)
        {
            if (workerPoint.childCount > 0) {
                workerPoint.GetChild(0).GetComponent<Worker>().isBabyMaking = false;
            }
        }

        if (timeToHaveBabyCurrent <= .1f) {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(workerItemPrefab, spawnItemsVector3, transform.rotation);
            HousingManager.instance.AddNewWorker();
            EconomyManager.instance.CheckDiscovery(1);
            CompleteFirstBabyTutorial();
        }

        timeToHaveBabyCurrent = timeToHaveBabyTotal;

        if (detectBabyMaking) {
            DetectBabyMaking();
        }
    }
}
