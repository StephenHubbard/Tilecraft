using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private GameObject sliderCanvas;
    [SerializeField] private Slider tileSlider;
    [SerializeField] private float startingCraftTime;
    [SerializeField] private float currentCraftTime;
    [SerializeField] public RecipeInfo recipeInfo;
    
    private int amountOfWorkers;
    public int amountLeftToCraft;
    public int startAmountToCraft;

    public bool hasCompleteRecipe = false;
    public bool hasWorkers = false;
    public bool isCrafting = false;

    public int currentCorrectItemsAcquiredForCrafting = 0;
    
    private AudioManager audioManager;

    private void Awake() {
        audioManager = FindObjectOfType<AudioManager>();
    }


    private void Start() {
        currentCraftTime = 10f;
    }

    private void Update() {
        if (currentCraftTime > 0 && hasCompleteRecipe) {
            tileSlider.value = currentCraftTime;
            currentCraftTime -= Time.deltaTime * amountOfWorkers;
        }

        if (currentCraftTime < .1f && hasCompleteRecipe && hasWorkers && isCrafting) {
            PopOutNewItemFromRecipe();
        } 
    }

    public void UpdateAmountLeftToCraft(int amountLeft) {
        amountLeftToCraft = amountLeft;
        startAmountToCraft = amountLeft;
    }

    public void CheckCanStartCrafting() {
        if (hasCompleteRecipe && hasWorkers && !isCrafting && amountLeftToCraft > 0) {
            StartCrafting();
            if (startAmountToCraft == amountLeftToCraft) {
                audioManager.Play(recipeInfo.craftingClipString);
            }
        } 
    }

    public void IncreaseWorkerCount() {
        amountOfWorkers += 1;
    }

    public void DecreaseWorkerCount() {
        amountOfWorkers -= 1;

        if (amountOfWorkers == 0) {
            hasWorkers = false;
        }

        CheckCanStartCrafting();
    }

    public void WorkerCountToZero() {
        amountOfWorkers = 0;
        hasWorkers = false;
    }

    private void StartCrafting() {
        sliderCanvas.SetActive(true);
        isCrafting = true;
        startingCraftTime = recipeInfo.recipeCraftTime;
        currentCraftTime = startingCraftTime;
        tileSlider.maxValue = startingCraftTime;

        foreach (var item in GetComponent<Tile>().workerPoints)
        {
            if (item.childCount > 0) {
                item.GetChild(0).GetComponent<Worker>().StartWorking();
            }
        }
    }

    public void DoneCrafting() {
        sliderCanvas.SetActive(false);
        hasCompleteRecipe = false;
        isCrafting = false;

        foreach (var item in GetComponent<Tile>().workerPoints)
        {
            if (item.childCount > 0) {
                item.GetChild(0).GetComponent<Worker>().StopWorking();
            }
        }

        audioManager.Play("Pop");
        GetComponent<Tile>().currentPlacedResources.Clear();
    }

    public void PopOutNewItemFromRecipe() {
        Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
        Instantiate(recipeInfo.itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
        isCrafting = false;
        amountLeftToCraft -= 1;

        if (amountLeftToCraft == 0) {
            DoneCrafting();
            GetComponent<Tile>().DoneCraftingDestroyItem();
        }

        CheckCanStartCrafting();
    }
}
