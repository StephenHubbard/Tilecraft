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
    [SerializeField] private GameObject childPrefab;
    [SerializeField] private GameObject[] workersAndPitchforks;

    public bool isBabyMaking = false;

    private void Start() {
        tileSlider.maxValue = 100;
        tileSlider.value = 100;

        HousingManager.instance.DetectHowManyHouses();
        HousingManager.instance.DetectTotalPopulation();
        workerPoints = GetComponentInParent<Tile>().workerPoints;
        timeToHaveBabyCurrent = timeToHaveBabyTotal;
        DetectBabyMaking();

        CompleteHouseTutorial();

        DeParentWorkersAndPitchforks();
    }

    private void CompleteHouseTutorial() {
        if (TutorialManager.instance.tutorialIndexNum == 5) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void CompleteFirstBabyTutorial() {
        if (TutorialManager.instance.tutorialIndexNum == 6) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void DeParentWorkersAndPitchforks() {
        if (DataPersistenceManager.instance.isLoadedGame) {
            foreach (var item in workersAndPitchforks)
            {
                Destroy(item.gameObject);
            }
            StartCoroutine(ChangeIsLoadedGameCo());
        } else {
            foreach (var item in workersAndPitchforks)
            {
                item.gameObject.transform.parent = null;
            }
        }

    }

    private IEnumerator ChangeIsLoadedGameCo() {
        yield return null;
        DataPersistenceManager.instance.isLoadedGame = false;
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
            Instantiate(childPrefab, spawnItemsVector3, transform.rotation);
            HousingManager.instance.AddNewWorker();
            // EconomyManager.instance.CheckDiscovery(1);
            CompleteFirstBabyTutorial();
        }

        timeToHaveBabyCurrent = timeToHaveBabyTotal;

        if (detectBabyMaking) {
            DetectBabyMaking();
        }
    }
}
