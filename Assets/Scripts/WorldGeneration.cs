using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize;
    [SerializeField] private GameObject tilePrefab;
    // [SerializeField] private GameObject[] tileTypePrefabs;
    [SerializeField] private TileInfo[] tileInfoScriptableObjects;
    [SerializeField] private Transform tileParent;
    
    private Grid grid;
    private int newTileSpawnDir = 1;
    private bool newTileSpawned = false;


    private void Start() {
        grid = new Grid(gridWidth, gridHeight, cellSize, new Vector3(0, 0));

        GenerateTiles();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
        }
    }

    private void GenerateTiles() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if (x == gridWidth / 2 && y == gridHeight / 2) {
                    Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                    GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                    GenWhichTileType(newTile.transform);

                    SpawnTileAbove(x, y);
                    SpawnTileBelow(x, y);
                    SpawnTileToRight(x, y);
                    SpawnTileToLeft(x, y);

                    grid.SetValue(gridLocation, 1);
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

    public void NewTile() {
        if (newTileSpawnDir == 1) {
            SpawnLeft();
        } else if (newTileSpawnDir == 2) {
            SpawnRight();
        } 

        // WARNING: will cause infinite loop crash if grid is full or close to full
        if (!newTileSpawned) {
            NewTile();
        } else {
            newTileSpawned = false;
        }
    }

    private void SpawnLeft() {
        for (int x = 0; x < grid.gridArray.GetLength(0); x ++) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if ((grid.GetValue(x + 1, y) == 1  || grid.GetValue(x - 1, y) == 1 || grid.GetValue(x, y + 1) == 1 || grid.GetValue(x, y - 1) == 1) && grid.GetValue(x, y) != 1) {
                    int randomNum = Random.Range(1, 10);

                    if (randomNum == 1) {
                        Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                        GenWhichTileType(newTile.transform);
                        grid.SetValue(gridLocation, 1);
                        newTileSpawned = true;
                        newTileSpawnDir = 2;
                        return;
                    } 
                }

            }
        }
    }

    private void SpawnRight() {
        for (int x = gridWidth - 1; x > 0; x--) {
            for (int y = 0; y < grid.gridArray.GetLength(1); y++) {
                
                if ((grid.GetValue(x + 1, y) == 1  || grid.GetValue(x - 1, y) == 1 || grid.GetValue(x, y + 1) == 1 || grid.GetValue(x, y - 1) == 1) && grid.GetValue(x, y) != 1) {
                    int randomNum = Random.Range(1, 10);

                    if (randomNum == 1) {
                        Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f;
                        GameObject newTile = Instantiate(tilePrefab, gridLocation, transform.rotation);
                        GenWhichTileType(newTile.transform);
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
