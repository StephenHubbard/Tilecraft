using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableItem : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string id;

    [SerializeField] public ItemInfo itemInfo;
    
    private GameObject tileHighlight;
    public Tile currentTile;
    public int amountLeft;

    private AudioManager audioManager;

    public bool CanPlaceOnTile = false;

    private void Awake() {
        tileHighlight = GameObject.Find("Highlighted Border - Square");
        audioManager = FindObjectOfType<AudioManager>();

        amountLeft = itemInfo.amountRecipeCanCreate;
    }

    private void Start() {
        tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
        GenerateGuid();

        StartCoroutine(CanPlaceItemOnTileDelay());
    }

    public IEnumerator CanPlaceItemOnTileDelay() {
        yield return new WaitForSeconds(.05f);
        CanPlaceOnTile = true;
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data) {
        
    }

    public void SaveData(ref GameData data) {
        if (!this.GetComponent<Population>()) {
            if (data.tilePositions.ContainsKey(id)) {
                data.tilePositions.Remove(id);
            }
            data.draggableItemPositions.Add(id, transform.position);
            data.draggableItemsItemInfo.Add(id, this.itemInfo);
        }
    }


    private void Update() {
        if (currentTile != null) {
            tileHighlight.GetComponent<SpriteRenderer>().enabled = true;
            tileHighlight.transform.position = currentTile.transform.position;
        }
    }

    public void UpdateAmountLeftToHarvest(int amountLeft) {
        this.amountLeft = amountLeft;
    }

    public void setActiveTile(GameObject hitTile) {
        currentTile = hitTile.GetComponent<Tile>();
    }

    public void tileHighlightOff() {
        tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
        currentTile = null;
    }

    public void CustomerOnTriggerExit2D() {
        // if (other.gameObject.GetComponent<Tile>() && currentTile == other.gameObject.GetComponent<Tile>()) {
            currentTile = null;
            tileHighlight.GetComponent<SpriteRenderer>().enabled = false;
        // }
    }

    public void PlaceItemOnTile(int amountInStack) {
        if (!CanPlaceOnTile || ToolTipManager.instance.isOverUI) { return; }

        for (int i = amountInStack; i > 0; i--)
        {
            // fridge
            if (currentTile.currentPlacedItem && currentTile.currentPlacedItem.GetComponent<Fridge>()) {
                if (gameObject.GetComponent<Food>()) {
                    currentTile.currentPlacedItem.GetComponent<Fridge>().IncreaseFoodAmount(gameObject.GetComponent<Food>().foodWorthAmount, GetComponent<Stackable>().amountOfChildItems);
                    AudioManager.instance.Play("Click");
                    Destroy(gameObject);
                    return;
                }
            }

            // Garbage
            if (currentTile.currentPlacedItem && currentTile.currentPlacedItem.GetComponent<PlacedItem>().itemInfo.itemName == "Garbage" && itemInfo.isResourceOnly) {
                    currentTile.currentPlacedItem.GetComponent<Garbage>().GarbageSpriteClosed();
                    AudioManager.instance.Play("Garbage");
                    Destroy(gameObject);
                    return;
                }

            // furnace
            if (currentTile.currentPlacedItem != null) {

                if (currentTile.currentPlacedItem.GetComponent<Furnace>() && itemInfo.isSmeltable && !currentTile.currentPlacedItem.GetComponent<Furnace>().occupiedWithResourceInFurnace) {
                    if (itemInfo.itemName == "Dead Worker" & !currentTile.currentPlacedItem.GetComponent<Furnace>().isAlter) {
                        DetermineExtraItems(i);
                        Destroy(gameObject);
                        return;
                    } else if (!currentTile.GetComponent<CraftingManager>().isCrafting && itemInfo.itemName == "Dead Worker" & currentTile.currentPlacedItem.GetComponent<Furnace>().isAlter) {
                        currentTile.currentPlacedItem.GetComponent<Furnace>().occupiedWithResourceInFurnace = true;
                        currentTile.currentPlacedItem.GetComponent<Furnace>().UpdateCurrentOccupiedResourceSprite(itemInfo);
                        currentTile.currentPlacedItem.GetComponent<Furnace>().StartSmelting(itemInfo, GetComponent<Stackable>().amountOfChildItems);
                        AudioManager.instance.Play("Click");
                        Destroy(gameObject);
                        return;
                    }

                    if (!currentTile.GetComponent<CraftingManager>().isCrafting && !currentTile.currentPlacedItem.GetComponent<Furnace>().isAlter) {
                        currentTile.currentPlacedItem.GetComponent<Furnace>().occupiedWithResourceInFurnace = true;
                        currentTile.currentPlacedItem.GetComponent<Furnace>().UpdateCurrentOccupiedResourceSprite(itemInfo);
                        currentTile.currentPlacedItem.GetComponent<Furnace>().StartSmelting(itemInfo, GetComponent<Stackable>().amountOfChildItems);
                        AudioManager.instance.Play("Click");
                        Destroy(gameObject);
                        return;
                    } else {
                        DetermineExtraItems(i);
                        Destroy(gameObject);
                        return;
                    }
                } else if (currentTile.currentPlacedItem.GetComponent<Furnace>() && itemInfo == currentTile.currentPlacedItem.GetComponent<Furnace>().currentlySmeltingItem) {
                        currentTile.GetComponent<CraftingManager>().amountLeftToCraft += GetComponent<Stackable>().amountOfChildItems;
                        currentTile.currentPlacedItem.GetComponent<Furnace>().amountLeftToSmelt += GetComponent<Stackable>().amountOfChildItems;
                        AudioManager.instance.Play("Click");
                        Destroy(gameObject);
                        return;
                    }
            }

            // worker
            if (itemInfo.name == "Worker") {
                if (currentTile.PlaceWorker(itemInfo.onTilePrefab, gameObject.GetComponent<Worker>().myHealth, gameObject.GetComponent<Worker>().myWorkingStrength, gameObject.GetComponent<Worker>().foodNeededToUpPickaxeStrengthCurrent, gameObject.GetComponent<Population>().currentLevel, GetComponent<Worker>().maxHealth)) {
                    if (i == 1) {
                        Destroy(gameObject);
                    }

                    if (currentTile.currentPlacedItem && currentTile.currentPlacedItem.GetComponent<House>()) {
                        currentTile.currentPlacedItem.GetComponent<House>().DetectBabyMaking();
                    }
                    continue;
                } else { 
                    currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();

                    DetermineExtraItems(i);
                    Destroy(gameObject);
                    return;
                }
            }

            // worker
            if (itemInfo.name == "Archer") {
                if (currentTile.PlaceArcher(itemInfo.onTilePrefab, gameObject.GetComponent<Archer>().myHealth, gameObject.GetComponent<Archer>().myCombatValue, gameObject.GetComponent<Archer>().foodNeededToUpCombatValue, gameObject.GetComponent<Population>().currentLevel, GetComponent<Archer>().maxHealth)) {
                    if (i == 1) {
                        Destroy(gameObject);
                    }
                    continue;
                } else { 
                    DetermineExtraItems(i);
                    Destroy(gameObject);
                    return;
                }
            }

            // worker
            if (itemInfo.name == "Knight") {
                if (currentTile.PlaceKnight(itemInfo.onTilePrefab, gameObject.GetComponent<Knight>().myHealth, gameObject.GetComponent<Knight>().myCombatValue, gameObject.GetComponent<Knight>().foodNeededToUpCombatValue, gameObject.GetComponent<Population>().currentLevel, GetComponent<Knight>().maxHealth)) {
                    if (i == 1) {
                        Destroy(gameObject);
                    }
                    continue;
                } else { 
                    DetermineExtraItems(i);
                    Destroy(gameObject);
                    return;
                }
            }

            // resources
            if (itemInfo.isResourceOnly && !currentTile.isOccupiedWithBuilding && !currentTile.GetComponent<CraftingManager>().isCrafting) {
                if (currentTile.PlaceResource(itemInfo.onTilePrefab)) {
                    currentTile.UpdateCurrentPlacedResourceList(itemInfo);
                    currentTile.isOccupiedWithResources = true;
                    if (i == 1) {
                        Destroy(gameObject);
                    }
                    currentTile.resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
                    continue;
                } else { 
                    currentTile.resourcePoints[0].GetChild(0).GetComponent<PlacedItem>().CheckForValidRecipe();
                    currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                    DetermineExtraItems(i);
                    Destroy(gameObject);
                    return;
                }
            }

            // main item
            if (currentTile != null && itemInfo.checkValidTiles(currentTile.GetComponent<Tile>().tileInfo) && !itemInfo.isResourceOnly) {

                if (currentTile.isOccupiedWithResources) {
                    foreach (Transform resource in currentTile.resourcePoints)
                    {
                        if (resource.childCount > 0) {
                            Destroy(resource.GetChild(0).gameObject);
                            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                            Instantiate(resource.GetChild(0).GetComponent<PlacedItem>().itemInfo.draggableItemPrefab, spawnItemsVector3, transform.rotation);
                        }
                    }
                }

                if (currentTile.currentPlacedItem && !currentTile.currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary) {
                    currentTile.GetComponent<CraftingManager>().DoneCrafting();
                    Destroy(currentTile.currentPlacedItem);
                } else if (currentTile.currentPlacedItem && currentTile.currentPlacedItem.GetComponent<PlacedItem>().itemInfo.isStationary) {
                    if (i - 1 == 0) {
                    DetermineExtraItems(i);
                    } else {
                        DetermineExtraItems(i);
                    }
                    Destroy(gameObject);
                    return;
                }

                currentTile.GetComponent<CraftingManager>().recipeInfo = null;
                GameObject thisItem = Instantiate(itemInfo.onTilePrefab, currentTile.transform.position, transform.rotation);
                thisItem.GetComponent<PlacedItem>().UpdateAmountLeftToHarvest(amountLeft);
                thisItem.transform.parent = currentTile.transform;
                currentTile.isOccupiedWithBuilding = true;
                currentTile.UpdateCurrentPlacedItem(itemInfo, thisItem);
                DetermineExtraItems(i - 1);
                currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                currentTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(amountLeft);
                if (itemInfo.isStationary || itemInfo.itemName == "Tower") {
                    AudioManager.instance.Play("Building Placement");
                    currentTile.InstantiateSmokePrefab();
                    currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                } else {
                    AudioManager.instance.Play("Click");
                }
                Destroy(gameObject);
                return;
            } else {
                if (i - 1 == 0) {
                    DetermineExtraItems(i);
                } else {
                    DetermineExtraItems(i);
                }
                Destroy(gameObject);
                return;
            }
        }

        currentTile.GetComponent<CraftingManager>().CheckCanStartCrafting();
    }

    private void DetermineExtraItems(int amountExtra) {

        for (int i = 0; i < amountExtra; i++)
        {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

            NewItemManager.instance.SpawnNewItem(spawnItemsVector3, itemInfo);
        }
    }


}
