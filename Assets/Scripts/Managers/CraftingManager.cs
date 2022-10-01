using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] public GameObject sliderCanvas;
    [SerializeField] public Slider tileSlider;
    [SerializeField] public Image sliderBackgroundColor;
    [SerializeField] private float startingCraftTime;
    [SerializeField] private float currentCraftTime;
    [SerializeField] public RecipeInfo recipeInfo;
    [SerializeField] private GameObject coinSellAnim;
    [SerializeField] public Color defaultGreen;
    [SerializeField] private RecipeInfo feedRecipeInfo;
    [SerializeField] private GameObject feedAnimPrefab;
    
    public int amountOfWorkers;
    public int totalWorkerStrength;
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
        defaultGreen = sliderBackgroundColor.color;
    }

    private void Start() {
        ResetStartingValuesSlider();
    }

    private void Update() {
        if (currentCraftTime > 0 && hasCompleteRecipe && isCrafting && hasWorkers) {
            tileSlider.value = currentCraftTime;
            currentCraftTime -= Time.deltaTime * totalWorkerStrength;
        }

        if (currentCraftTime < .1f && hasCompleteRecipe && hasWorkers && isCrafting) {
            PopOutNewItemFromRecipe();

            if (recipeInfo && recipeInfo.itemInfo.itemName == "Tower") {
                ResetStartingValuesSlider();
            }
        } 
    }

    public void ResetStartingValuesSlider() {
        if (GetComponent<Tile>()) {
            DoneCrafting();
        }
        tileSlider.value = currentCraftTime;
        tileSlider.maxValue = 100;
        tileSlider.value = 100;
        sliderBackgroundColor.color = defaultGreen;
    }

    public void CompleteFirstStepTutorial() {
        if (TutorialManager.instance.tutorialIndexNum == 0) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void CompleteFarmTutorial() {
        if (recipeInfo && recipeInfo.itemInfo.itemName == "Farm" && TutorialManager.instance.tutorialIndexNum == 1) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void CraftWorkerTutorial() {
        if (recipeInfo && recipeInfo.itemInfo.itemName == "Worker" && TutorialManager.instance.tutorialIndexNum == 7) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    public void UpdateAmountLeftToCraft(int amountLeft) {
        amountLeftToCraft = amountLeft;
        startAmountToCraft = amountLeft;
    }

    public void CheckCanStartCrafting() {
        if (GetComponent<FeedOnTile>().CheckIfCanEat()) {
            hasCompleteRecipe = true;
            foreach (var resourcePoint in GetComponent<Tile>().resourcePoints)
            {
                if (resourcePoint.childCount > 0) {
                    recipeInfo = feedRecipeInfo;
                    break;
                }
            }

            foreach (var workerPoint in GetComponent<Tile>().workerPoints)
            {
                if (workerPoint.childCount > 0) {
                    hasWorkers = true;
                    totalWorkerStrength++;
                }
            }
        }

        if (GetComponent<Tile>().currentPlacedItem) {
            hasCompleteRecipe = true;
        }

        if (GetComponent<Tile>().currentPlacedItem && GetComponent<Tile>().currentPlacedItem.GetComponent<Tower>()) {
            hasCompleteRecipe = true;
            recipeInfo = GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.recipeInfo;
            amountLeftToCraft = 1;
            startAmountToCraft = 1;
        }

        if (hasCompleteRecipe && hasWorkers && !isCrafting && amountLeftToCraft > 0 && recipeInfo && !recipeInfo.requiresFurnace) {
            if (CheckIfTileHasEnemies() == false) {
                StartCrafting();
                if (startAmountToCraft == amountLeftToCraft) {
                    AudioManager.instance.Play(recipeInfo.craftingClipString);
                }
            }

        } else if (hasCompleteRecipe && hasWorkers && !isCrafting && amountLeftToCraft > 0 && recipeInfo && recipeInfo.requiresFurnace) {
            if (GetComponent<Tile>().currentPlacedItem && GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>()) {
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
            if (worker.childCount > 0 && worker.GetChild(0).GetComponent<Worker>()) {
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

        StartCoroutine(FindWorkersEndOfFrame());
    }

    private IEnumerator FindWorkersEndOfFrame() {
        yield return new WaitForEndOfFrame();

        foreach (var item in GetComponent<Tile>().workerPoints)
        {
            if (item.childCount > 0 && item.GetChild(0).GetComponent<Worker>()) {
                amountOfWorkers++;
            }
        }

        if (amountOfWorkers > 0) {
            hasWorkers = true;
        } else {
            DoneCrafting();
            ResetStartingValuesSlider();
        }
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
            if (item.childCount > 0 && item.GetChild(0).GetComponent<Worker>())
            {
                item.GetChild(0).GetComponent<Worker>().StartWorking();
            }
        }
    }

    public void DoneCrafting() {
        sliderCanvas.SetActive(false);
        hasCompleteRecipe = false;
        isCrafting = false;
        if (GetComponent<Tile>().itemInfo) {
            GetComponent<Tile>().itemInfo = null;
        }
        if (GetComponent<Tile>().currentPlacedItem && amountLeftToCraft != 0) {
            GetComponent<Tile>().itemInfo = GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo;
        }

        foreach (var item in GetComponent<Tile>().workerPoints)
        {
            if (item.childCount > 0 && item.GetChild(0).GetComponent<Worker>()) {
                item.GetChild(0).GetComponent<Worker>().StopWorking();
            }
        }

        GetComponent<Tile>().currentPlacedResources.Clear();

        if (!hasWorkers) {
            if (GetComponent<Tile>().currentPlacedItem && GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>()) {
                GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>().occupiedWithResourceInFurnace = false;
                GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>().HideOccupiedSpriteContainer();
                GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>().AbandonSmelting();
            }
        }

        
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
        CompleteFarmTutorial();
        CraftWorkerTutorial();


        // Feed
        if (recipeInfo == feedRecipeInfo) {
            Population popToFeed = null;
            foreach (var resourcePoint in GetComponent<Tile>().resourcePoints)
            {
                if (resourcePoint.childCount > 0) {
                    GameObject thisFeedAnim = Instantiate(feedAnimPrefab, resourcePoint.transform.position, transform.rotation);
                    foreach (var workerPoint in GetComponent<Tile>().workerPoints)
                    {
                        if (workerPoint.childCount > 0) {
                            popToFeed = workerPoint.GetChild(0).GetComponent<Population>();
                            thisFeedAnim.GetComponent<FoodAnim>().FeedPopulation(popToFeed);
                            break;
                        }
                    }
                }
            }
        }

        // EconomyManager.instance.CheckDiscovery(recipeInfo.itemInfo.coinValue);
        Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
        if (recipeInfo.itemInfo.itemName == "XP") {
            spawnItemsVector3 = transform.position + new Vector3(0, 1, 0);
        }
        if (recipeInfo != feedRecipeInfo) {
            GameObject craftedItem = Instantiate(recipeInfo.itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
            // craft x2 if it's a weapon
            if (craftedItem.GetComponent<Weapon>()) {
                Instantiate(recipeInfo.itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
            }
            recipeInfo.itemInfo.NextInLineToDiscover();
            AutoSellCraftedItem(craftedItem);

        } 
        if (recipeInfo.itemInfo.itemName != "XP" && recipeInfo.itemInfo.itemName != "Feed") {
            encyclopedia.AddItemToDiscoveredList(recipeInfo.itemInfo, true, false);
        }
        ToDoManager.instance.CraftedItemTakeOffToDoList(recipeInfo.itemInfo);
        StartCoroutine(Encyclopedia.instance.CraftedNewItemToggleCraftButton());

        foreach (var item in GetComponent<Tile>().resourcePoints)
        {
            if (item.childCount > 0) {
                Destroy(item.GetChild(0).gameObject);
            }
        }

        
        Encyclopedia.instance.CraftedDiscoveredItem(recipeInfo.itemInfo);

        isCrafting = false;
        amountLeftToCraft -= 1;

        if (amountLeftToCraft == 0) {
            recipeInfo = null;
            GetComponent<Tile>().DoneCraftingDestroyItem();
            DoneCrafting();
        }

        if (transform.GetComponent<Tile>().currentPlacedItem && transform.GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>()) {
            transform.GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>().UpdateFurnaceAmountLeft();
        }

        if (GetComponent<Tile>().currentPlacedItem && amountLeftToCraft == 0 && GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>()) {
            GetComponent<Tile>().currentPlacedItem.GetComponent<Furnace>().AbandonSmelting();
        }


        CheckCanStartCrafting();

        CompleteFirstStepTutorial();
    }

    private void AutoSellCraftedItem(GameObject craftedItem) {
        if(GetComponent<Tile>().isAutoSellOn) {
            craftedItem.GetComponent<Stackable>().isSellable = true;
            // EconomyManager.instance.SellItem(craftedItem, craftedItem.GetComponent<DraggableItem>().itemInfo.coinValue, 1);
        }
    }
}
