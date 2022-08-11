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
    [SerializeField] private int amountOfStartingTiles = 20;
    [SerializeField] private ItemInfo[] spawnableTileItems;
    
    private Grid grid;
    private int newTileSpawnDir = 1;
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
        GenerateTiles();
        GenStartingTilesAroundCenter();
    }

    public Grid ReturnGrid() {
        return grid;
    }

    public ItemInfo[] ReturnSpawnableItems() {
        return spawnableTileItems;
    }

    private void GenerateTiles() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if (x == gridWidth / 2 && y == gridHeight / 2) {
                    Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                    // always spawns grass tile at center
                    GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                    GameObject tileType = Instantiate(tileInfoScriptableObjects[0].tilePrefab, newTile.transform.position, transform.rotation);
                    GameObject startingTower = Instantiate(towerPrefab, newTile.transform.position, transform.rotation);
                    startingTower.transform.parent = newTile.transform;
                    newTile.GetComponent<Tile>().UpdateCurrentPlacedItem(towerPrefab.GetComponent<PlacedItem>().itemInfo, startingTower);
                    newTile.GetComponent<Tile>().isOccupiedWithBuilding = true;
                    tileType.transform.parent = newTile.transform;
                    tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[0];

                    // SpawnTileAbove(x, y);
                    // SpawnTileBelow(x, y);
                    // SpawnTileToRight(x, y);
                    // SpawnTileToLeft(x, y);

                    grid.SetValue(gridLocation, 1);
                }
            }
        }
    }

    private void GenStartingTilesAroundCenter() {
        for (int i = 0; i < amountOfStartingTiles; i++)
        {
            NewTile(i);
        }
    }

    private void GenWhichTileType(Transform newTile) {
        int randomNum = Random.Range(0, tileInfoScriptableObjects.Length);
        GameObject tileType = Instantiate(tileInfoScriptableObjects[randomNum].tilePrefab, newTile.transform.position, transform.rotation);
        tileType.transform.parent = newTile;
        tileType.GetComponentInParent<Tile>().tileInfo = tileInfoScriptableObjects[randomNum];
    }


    private void SpawnTileAbove(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x, y + 1) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        GenWhichTileType(newTile.transform);

        grid.SetValue(gridLocation, 1);
    }

    private void SpawnTileBelow(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x, y - 1) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        GenWhichTileType(newTile.transform);
        grid.SetValue(gridLocation, 1);
    }

    private void SpawnTileToRight(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x + 1, y) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        GenWhichTileType(newTile.transform);
        grid.SetValue(gridLocation, 1);
    }

    private void SpawnTileToLeft(int x, int y) {
        Vector3 gridLocation = grid.GetWorldPosition(x - 1, y) + new Vector3(cellSize, cellSize) * .5f;
        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
        GenWhichTileType(newTile.transform);
        grid.SetValue(gridLocation, 1);
    }

    public void NewTile(int i) {
        if (newTileSpawnDir == 1) {
            SpawnLeft(i);
        } else if (newTileSpawnDir == 2) {
            SpawnRight(i);
        } 

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

    private void SpawnLeft(int i) {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if ((grid.GetValue(x + 1, y) == 1  || grid.GetValue(x - 1, y) == 1 || grid.GetValue(x, y + 1) == 1 || grid.GetValue(x, y - 1) == 1) && grid.GetValue(x, y) != 1) {
                    int randomNum = Random.Range(1, 10);

                    if (randomNum == 1) {
                        Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                        GenWhichTileType(newTile.transform);

                        int doesSpawnStartingItemNum = Random.Range(1, 3);

                        if (i < 20) {
                            doesSpawnStartingItemNum = 1;
                        }

                        if (doesSpawnStartingItemNum == 1) {

                            List<ItemInfo> potentialItems = new List<ItemInfo>();
                            
                            foreach (var potentialItem in spawnableTileItems)
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

                        }

                        grid.SetValue(gridLocation, 1);
                        newTileSpawned = true;
                        newTileSpawnDir = 2;
                        return;
                    } 
                }

            }
        }
    }

    private void SpawnRight(int i) {
        for (int x = gridWidth - 1; x > 0; x--) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if ((grid.GetValue(x + 1, y) == 1  || grid.GetValue(x - 1, y) == 1 || grid.GetValue(x, y + 1) == 1 || grid.GetValue(x, y - 1) == 1) && grid.GetValue(x, y) != 1) {
                    int randomNum = Random.Range(1, 10);

                    if (randomNum == 1) {
                        Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                        GenWhichTileType(newTile.transform);

                        int doesSpawnStartingItemNum = Random.Range(1, 3);

                        if (i < 20) {
                            doesSpawnStartingItemNum = 1;
                        }

                        if (doesSpawnStartingItemNum == 1) {

                            List<ItemInfo> potentialItems = new List<ItemInfo>();
                            
                            foreach (var potentialItem in spawnableTileItems)
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

                        }


                        grid.SetValue(gridLocation, 1);
                        newTileSpawned = true;
                        newTileSpawnDir = 1;
                        return;
                    } 
                }

            }
        }
    }
}
