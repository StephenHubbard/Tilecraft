using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class WorldGeneration : MonoBehaviour
{

    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] public float cellSize;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private TileInfo[] tileInfoScriptableObjects;
    [SerializeField] private Transform tileParent;
    [SerializeField] private GameObject towerPrefab;
    [Header("NewTile() iterations, not exactly amount of tile")]
    [SerializeField] private int amountOfStartingTiles = 20;

    [SerializeField] private ItemInfo[] spawnableTileItemsTierOne;
    [SerializeField] private ItemInfo[] spawnableTileItemsTierTwo;
    [SerializeField] private ItemInfo[] spawnableTileItemsTierThree;
    [SerializeField] private LayerMask tileLayerMask = new LayerMask();

    [SerializeField] private int tierTwoMinX;
    [SerializeField] private int tierTwoMaxX;
    [SerializeField] private int tierTwoMinY;
    [SerializeField] private int tierTwoMaxY;

    [SerializeField] private int tierThreeMinX;
    [SerializeField] private int tierThreeMaxX;
    [SerializeField] private int tierThreeMinY;
    [SerializeField] private int tierThreeMaxY;

    
    private Grid grid;
    private bool newTileSpawned = false;

    public static WorldGeneration instance;

    private AudioManager audioManager;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        audioManager = FindObjectOfType<AudioManager>();

        grid = new Grid(gridWidth, gridHeight, cellSize, new Vector3(0, 0));
    }


    private void Start() {
        GenerateMidTile();
        GenStartingTilesAroundCenter();
        CreateHolesNearCenter();
        SpawnTierThreeItems();
        SpawnTierTwoItems();
        CheckThereIsEnoughSpreadResources();

        if (!InputManager.instance.isOnMainMenu) {
            HousingManager.instance.SpawnStartingThreeWorkers();
        }
    }

    public Grid ReturnGrid() {
        return grid;
    }

    public ItemInfo[] ReturnSpawnableItemsTierOne() {
        return spawnableTileItemsTierOne;
    }

    public ItemInfo[] ReturnSpawnableItemsTierTwo() {
        return spawnableTileItemsTierTwo;
    }

    public ItemInfo[] ReturnSpawnableItemsTierThree() {
        return spawnableTileItemsTierThree;
    }

    public bool ReturnTierTwoBoundries(int x, int y) {
        if ((x >= tierTwoMinX && x <= tierTwoMaxX) && (y >= tierTwoMinY && y <= tierTwoMaxY)) 
        { 
            return false;
        } 
        return true;
    }

    public bool ReturnTierThreeBoundries(int x, int y) {
        if ((x >= tierThreeMinX && x <= tierThreeMaxX) && (y >= tierThreeMinY && y <= tierThreeMaxY)) 
        { 
            return false;
        } 
        return true;
    }

    private void CheckThereIsEnoughSpreadResources() {
        PlacedItem[] allPlacedItemsOfThisType = FindObjectsOfType<PlacedItem>();

        foreach (var item in spawnableTileItemsTierTwo)
        {
            int amountOfThisType = 0;

            foreach (var placedItem in allPlacedItemsOfThisType)
            {
                if (item == placedItem.itemInfo) {
                    amountOfThisType++;
                }
            }

            if (item.itemName == "Copper Node" || item.itemName == "Sand Pile") {
                if (amountOfThisType < 15) {
                    SpawnSpecificItem(item);
                }
            }
        }
        
        foreach (var item in spawnableTileItemsTierThree)
        {
            int amountOfThisType = 0;

            foreach (var placedItem in allPlacedItemsOfThisType)
            {
                if (item == placedItem.itemInfo) {
                    amountOfThisType++;
                }
            }

            if (item.itemName == "Oil Spill") {
                if (amountOfThisType < 9) {
                    SpawnSpecificItem(item);
                }
            }
        }
    }

    private void SpawnSpecificItem(ItemInfo itemInfo) {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {

                // non spawnable area for tier 3 items
                if (!(x > tierThreeMinX && x < tierThreeMaxX) && !(y > tierThreeMinY && y < tierThreeMaxY)) 
                { 
                    if (grid.GetValue(x, y) == 1) {
                        Vector3 worldPos = grid.GetWorldPosition(x, y);
                        worldPos.x = worldPos.x + 1f;
                        worldPos.y = worldPos.y + 1f;

                        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, tileLayerMask);

                        if (hit && !hit.transform.GetComponent<Tile>().currentPlacedItem) {
                            int doesSpawnStartingItemNum = Random.Range(1, 3);

                            if (doesSpawnStartingItemNum == 1) {

                                List<ItemInfo> potentialItems = new List<ItemInfo>();
                                
                                foreach (var potentialItem in spawnableTileItemsTierTwo)
                                {
                                    foreach (var validLandTile in potentialItem.tileInfoValidLocations)
                                    {
                                        if (validLandTile == hit.transform.GetComponent<Tile>().tileInfo) {
                                            potentialItems.Add(potentialItem);
                                        }
                                    }
                                }

                                int whichPrefabToSpawnNum = Random.Range(0, potentialItems.Count);
                                
                                if (potentialItems.Count == 0) { continue; }

                                GameObject startingPlacedItem = Instantiate(potentialItems[whichPrefabToSpawnNum].onTilePrefab, hit.transform.position, transform.rotation);
                                startingPlacedItem.transform.parent = hit.transform;
                                hit.transform.GetComponent<Tile>().UpdateCurrentPlacedItem(startingPlacedItem.GetComponent<PlacedItem>().itemInfo, startingPlacedItem);
                                hit.transform.GetComponent<Tile>().isOccupiedWithBuilding = true;
                                hit.transform.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(startingPlacedItem.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

                                if (startingPlacedItem.GetComponent<OrcRelic>()) {
                                    startingPlacedItem.GetComponent<OrcRelic>().spawnEnemies = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SpawnTierTwoItems() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {

                // non spawnable area for tier 2 items
                if (!(x > tierTwoMinX && x < tierTwoMaxX) && !(y > tierTwoMinY && y < tierTwoMaxY)) 
                { 
                    if (grid.GetValue(x, y) == 1) {
                        Vector3 worldPos = grid.GetWorldPosition(x, y);
                        worldPos.x = worldPos.x + 1f;
                        worldPos.y = worldPos.y + 1f;

                        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, tileLayerMask);

                        if (hit && !hit.transform.GetComponent<Tile>().currentPlacedItem) {
                            int doesSpawnStartingItemNum = Random.Range(1, 4);

                            if (doesSpawnStartingItemNum == 1) {

                                List<ItemInfo> potentialItems = new List<ItemInfo>();
                                
                                foreach (var potentialItem in spawnableTileItemsTierTwo)
                                {
                                    foreach (var validLandTile in potentialItem.tileInfoValidLocations)
                                    {

                                        if (validLandTile == hit.transform.GetComponent<Tile>().tileInfo) {
                                            potentialItems.Add(potentialItem);
                                        }
                                    }
                                }
                                int whichPrefabToSpawnNum = Random.Range(0, potentialItems.Count);

                                GameObject startingPlacedItem = Instantiate(potentialItems[whichPrefabToSpawnNum].onTilePrefab, hit.transform.position, transform.rotation);
                                startingPlacedItem.transform.parent = hit.transform.transform;
                                hit.transform.GetComponent<Tile>().UpdateCurrentPlacedItem(startingPlacedItem.GetComponent<PlacedItem>().itemInfo, startingPlacedItem);
                                hit.transform.GetComponent<Tile>().isOccupiedWithBuilding = true;
                                hit.transform.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(startingPlacedItem.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

                                if (startingPlacedItem.GetComponent<OrcRelic>()) {
                                    startingPlacedItem.GetComponent<OrcRelic>().spawnEnemies = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SpawnTierThreeItems() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {

                // non spawnable area for tier 3 items
                if (!(x > tierThreeMinX && x < tierThreeMaxX) && !(y > tierThreeMinY && y < tierThreeMaxY)) 
                { 
                    if (grid.GetValue(x, y) == 1) {
                        Vector3 worldPos = grid.GetWorldPosition(x, y);
                        worldPos.x = worldPos.x + 1f;
                        worldPos.y = worldPos.y + 1f;

                        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, tileLayerMask);

                        if (hit && !hit.transform.GetComponent<Tile>().currentPlacedItem) {
                            int doesSpawnStartingItemNum = Random.Range(1, 3);

                            if (doesSpawnStartingItemNum == 1) {

                                List<ItemInfo> potentialItems = new List<ItemInfo>();
                                
                                foreach (var potentialItem in spawnableTileItemsTierThree)
                                {
                                    foreach (var validLandTile in potentialItem.tileInfoValidLocations)
                                    {

                                        if (validLandTile == hit.transform.GetComponent<Tile>().tileInfo) {
                                            potentialItems.Add(potentialItem);
                                        }
                                    }
                                }
                                int whichPrefabToSpawnNum = Random.Range(0, potentialItems.Count);
                                
                                if (potentialItems.Count == 0) { continue; }

                                GameObject startingPlacedItem = Instantiate(potentialItems[whichPrefabToSpawnNum].onTilePrefab, hit.transform.position, transform.rotation);
                                startingPlacedItem.transform.parent = hit.transform.transform;
                                hit.transform.GetComponent<Tile>().UpdateCurrentPlacedItem(startingPlacedItem.GetComponent<PlacedItem>().itemInfo, startingPlacedItem);
                                hit.transform.GetComponent<Tile>().isOccupiedWithBuilding = true;
                                hit.transform.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(startingPlacedItem.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

                                if (startingPlacedItem.GetComponent<OrcRelic>()) {
                                    startingPlacedItem.GetComponent<OrcRelic>().spawnEnemies = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenerateMidTile() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if (x == gridWidth / 2 && y == gridHeight / 2) {
                    Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                    // always spawns grass and tower tile at center
                    GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                    
                    GameObject tileType = Instantiate(tileInfoScriptableObjects[0].tilePrefab, newTile.transform.position, transform.rotation);
                    GameObject startingTower = Instantiate(towerPrefab, newTile.transform.position, transform.rotation);
                    startingTower.transform.parent = newTile.transform;
                    // newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(towerPrefab.GetComponent<PlacedItem>().itemInfo, startingTower);
                    // newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
                    // newTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(towerPrefab.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);
                    tileType.transform.parent = newTile.transform;
                    tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[0];
                    grid.SetValue(gridLocation, 1);

                    SpawnFourAdjacentTiles(x, y);
                    
                    StartCoroutine(DestroyStartingTower(startingTower));
                }
            }
        }
    }

    private IEnumerator DestroyStartingTower(GameObject startingTower) {
        yield return new WaitForEndOfFrame();
        Destroy(startingTower);
    }

    private void SpawnFourAdjacentTiles(int x, int y) {
        SpawnTileAbove(x, y);
        SpawnTileBelow(x, y);
        SpawnTileToRight(x, y);
        SpawnTileToLeft(x, y);
    }

    private void SpawnTileAbove(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x, y + 1) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        
        GameObject tileType = Instantiate(tileInfoScriptableObjects[0].tilePrefab, newTile.transform.position, transform.rotation);
        GameObject startingItem = Instantiate(spawnableTileItemsTierOne[4].onTilePrefab, newTile.transform.position, transform.rotation);
        startingItem.transform.parent = newTile.transform;
        newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(spawnableTileItemsTierOne[4].onTilePrefab.GetComponent<PlacedItem>().itemInfo, startingItem);
        newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
        tileType.transform.parent = newTile.transform;
        tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[0];
        newTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(spawnableTileItemsTierOne[4].onTilePrefab.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

        grid.SetValue(gridLocation, 1);
    }

    private void SpawnTileBelow(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x, y - 1) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        
        GameObject tileType = Instantiate(tileInfoScriptableObjects[1].tilePrefab, newTile.transform.position, transform.rotation);
        GameObject startingItem = Instantiate(spawnableTileItemsTierOne[1].onTilePrefab, newTile.transform.position, transform.rotation);
        startingItem.transform.parent = newTile.transform;
        newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(spawnableTileItemsTierOne[1].onTilePrefab.GetComponent<PlacedItem>().itemInfo, startingItem);
        newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
        tileType.transform.parent = newTile.transform;
        tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[1];
        newTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(spawnableTileItemsTierOne[1].onTilePrefab.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

        grid.SetValue(gridLocation, 1);
    }

    private void SpawnTileToRight(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x + 1, y) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        
        GameObject tileType = Instantiate(tileInfoScriptableObjects[2].tilePrefab, newTile.transform.position, transform.rotation);
        GameObject startingItem = Instantiate(spawnableTileItemsTierOne[3].onTilePrefab, newTile.transform.position, transform.rotation);
        startingItem.transform.parent = newTile.transform;
        newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(spawnableTileItemsTierOne[3].onTilePrefab.GetComponent<PlacedItem>().itemInfo, startingItem);
        newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
        tileType.transform.parent = newTile.transform;
        tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[2];
        newTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(spawnableTileItemsTierOne[3].onTilePrefab.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

        grid.SetValue(gridLocation, 1);
    }

    private void SpawnTileToLeft(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x - 1, y) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        
        GameObject tileType = Instantiate(tileInfoScriptableObjects[3].tilePrefab, newTile.transform.position, transform.rotation);
        GameObject startingItem = Instantiate(spawnableTileItemsTierOne[5].onTilePrefab, newTile.transform.position, transform.rotation);
        startingItem.transform.parent = newTile.transform;
        newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(spawnableTileItemsTierOne[5].onTilePrefab.GetComponent<PlacedItem>().itemInfo, startingItem);
        newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
        tileType.transform.parent = newTile.transform;
        tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[3];
        newTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(spawnableTileItemsTierOne[5].onTilePrefab.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

        grid.SetValue(gridLocation, 1);
    }

    private void GenStartingTilesAroundCenter() {
        for (int i = 0; i < amountOfStartingTiles; i++)
        {
            NewTile(i);
        }
    }

    private void CreateHolesNearCenter() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                if (grid.GetValue(x, y) == 1) {
                    Vector3 worldPos = grid.GetWorldPosition(x, y);
                    worldPos.x = worldPos.x + 1f;
                    worldPos.y = worldPos.y + 1f;

                    RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 100f, tileLayerMask);

                    // don't destroy middle tile
                    if (hit && (x != gridWidth / 2 && y != gridHeight / 2)) {
                        int randomNum = Random.Range(0, 7);

                        if (randomNum == 1) {
                            Destroy(hit.transform.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void GenWhichTileType(Transform newTile) {
        int randomNum = Random.Range(0, tileInfoScriptableObjects.Length);
        GameObject tileType = Instantiate(tileInfoScriptableObjects[randomNum].tilePrefab, newTile.transform.position, transform.rotation);
        tileType.transform.parent = newTile;
        tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[randomNum];
    }


    public void NewTile(int i) {
        SpawnTile(i);

        // WARNING: will cause infinite loop crash if grid is full or close to full
        if (!newTileSpawned) {
            NewTile(i);
        } else {
            newTileSpawned = false;
        }
    }

    public void PlayNewTileSound() {
        audioManager.Play("Tile Placement");
    }

    private void SpawnTile(int i) {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                GridTileLogic(x, y, i);
            }
        }
    }


    private void GridTileLogic(int x, int y, int i) {
        if ((grid.GetValue(x + 1, y) == 1  || grid.GetValue(x - 1, y) == 1 || grid.GetValue(x, y + 1) == 1 || grid.GetValue(x, y - 1) == 1) && grid.GetValue(x, y) != 1) {
                    int randomNum = Random.Range(1, 20);

                    if (randomNum == 1) {
                        Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                        
                        GenWhichTileType(newTile.transform);

                        int doesSpawnStartingItemNum = Random.Range(1, 4);

                        if (i < 20) {
                            doesSpawnStartingItemNum = 1;
                        }

                        if (doesSpawnStartingItemNum == 1) {

                            List<ItemInfo> potentialItems = new List<ItemInfo>();
                            
                            foreach (var potentialItem in spawnableTileItemsTierOne)
                            {
                                foreach (var validLandTile in potentialItem.tileInfoValidLocations)
                                {

                                    if (validLandTile == newTile.GetComponent<Tile>().tileInfo) {
                                        potentialItems.Add(potentialItem);
                                    }
                                }
                            }
                            int whichPrefabToSpawnNum = Random.Range(0, potentialItems.Count);

                            GameObject startingPlacedItem = Instantiate(potentialItems[whichPrefabToSpawnNum].onTilePrefab, newTile.transform.position, transform.rotation);
                            startingPlacedItem.transform.parent = newTile.transform;
                            newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(startingPlacedItem.GetComponent<PlacedItem>().itemInfo, startingPlacedItem);
                            newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
                            newTile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(startingPlacedItem.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);

                            if (startingPlacedItem.GetComponent<OrcRelic>()) {
                                startingPlacedItem.GetComponent<OrcRelic>().spawnEnemies = true;
                            }
                        }

                        grid.SetValue(gridLocation, 1);
                        newTileSpawned = true;

                        return;
                    } 
                }
    }
}
