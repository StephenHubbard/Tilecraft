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
    [SerializeField] private Transform tileParent;
    [SerializeField] private GameObject[] allTiles;

    private Grid grid;


    private void Start() {
        grid = new Grid(gridWidth, gridHeight, cellSize, new Vector3(0, 0));

        GenerateTiles();
    }

    private void Update() {
        // if (Input.GetMouseButtonDown(0)) {
        //     grid.SetValue(UtilsClass.GetMouseWorldPosition(), 1);
        // }

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
                    ParentTheTile(newTile.transform);

                    Vector3 gridLocationAbove = grid.GetWorldPosition(x, y + 1) + new Vector3(cellSize, cellSize) * .5f;
                    GameObject newTileAbove = Instantiate(tilePrefab, gridLocationAbove, transform.rotation);
                    ParentTheTile(newTileAbove.transform);
                    
                    grid.SetValue(x, y, 1);
                }

            }
        }
    }

    private void ParentTheTile(Transform newTile) {
        newTile.parent = tileParent;
    }

}
