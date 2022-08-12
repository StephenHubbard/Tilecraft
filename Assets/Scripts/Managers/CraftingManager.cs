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
    private int totalWorkerStrength;
    public int amountLeftToCraft;
    public int startAmountToCraft;

    public bool hasCompleteRecipe = false;
    public bool hasWorkers = false;
    public bool isCrafting = false;

    public int currentCorrectItemsAcquiredForCrafting = 0;
    
    private AudioManager audioManager;
    private Encyclopedia encyclopedia;

    private void Awake() {
        audioManager = FindObjectOfType<AudioManager>();
        encyclopedia = FindObjectOfType<Encyclopedia>();
    }


    private void Start() {
        // currentCraftTime = 10f;
    }

    private void Update() {
        if (currentCraftTime > 0 && hasCompleteRecipe && isCrafting && hasWorkers) {
            tileSlider.value = currentCraftTime;
            currentCraftTime -= Time.deltaTime * totalWorkerStrength;
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
        if (hasCompleteRecipe && hasWorkers && !isCrafting && amountLeftToCraft > 0 && !recipeInfo.requiresFurnace) {
            if (CheckIfTileHasEnemies() == false) {
                StartCrafting();
                if (startAmountToCraft == amountLeftToCraft) {
                    audioManager.Play(recipeInfo.craftingClipString);
                }
            }

        } else if (hasCompleteRecipe && hasWorkers && !isCrafting && amountLeftToCraft > 0 && recipeInfo.requiresFurnace) {
            if (GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>()) {
                StartCrafting();
                if (startAmountToCraft == amountLeftToCraft) {
                    audioManager.Play(recipeInfo.craftingClipString);
                }
            }
        }
    }

    private bool CheckIfTileHasEnemies() {
        if(GetComponent<Tile>().currentPlacedItem && GetComponent<Tile>().currentPlacedItem.GetComponent<OrcRelic>()) {
            if (GetComponent<Tile>().currentPlacedItem.GetComponent<OrcRelic>().hasEnemies) {
                return true;
            } 
        } 

        return false;
    }

    

    public void IncreaseWorkerCount() {
        amountOfWorkers += 1;
        int newTotalStrength = 0;

        foreach (var worker in GetComponent<Tile>().workerPoints)
        {
            if (worker.childCount > 0) {
                newTotalStrength += worker.GetChild(0).GetComponent<Worker>().myWorkingStrength;
            }
        }

        totalWorkerStrength = newTotalStrength;
    }

    public void DecreaseWorkerCount() {
        amountOfWorkers -= 1;

        if (amountOfWorkers == 0) {
            hasWorkers = false;
        }

        if (amountOfWorkers < 0) {
            amountOfWorkers = 0;
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

        amountOfWorkers = 0;

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

    public void AllWorkersHaveDiedCheck() {
        StartCoroutine(AllWorkersHaveDiedCheckCo());
    }

    private IEnumerator AllWorkersHaveDiedCheckCo() {
        yield return new WaitForEndOfFrame();

        int potentialWorkers = 3;

        foreach (var workerPoint in GetComponent<Tile>().workerPoints)
        {
            if (workerPoint.childCount > 0) {
                continue;
            } else {
                potentialWorkers--;
            }
        }

        if (potentialWorkers == 0) {
            hasWorkers = false;
        }
    }

    public void PopOutNewItemFromRecipe() {
        Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
        Instantiate(recipeInfo.itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
        encyclopedia.AddItemToDiscoveredList(recipeInfo.itemInfo);

        foreach (var recipe in recipeInfo.itemInfo.recipeInfo.neededRecipeItems)
        {
        encyclopedia.AddItemToDiscoveredList(recipe);
        }
        
        isCrafting = false;
        amountLeftToCraft -= 1;

        if (amountLeftToCraft == 0) {
            DoneCrafting();
            GetComponent<Tile>().DoneCraftingDestroyItem();
        }

        if (transform.GetComponent<Tile>().currentPlacedItem && transform.GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>()) {
            transform.GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>().UpdateFurnaceAmountLeft();
        }

        CheckCanStartCrafting();
    }
}
