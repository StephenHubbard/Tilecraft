using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGeneration : MonoBehaviour
{
    [SerializeField] private GameObject[] cloudPrefabs;
    [SerializeField] private Transform cloudParent;
    [SerializeField] private int amountOfClouds;
    private WorldGeneration worldGeneration;
    private Grid grid;

    private void Awake() {
        worldGeneration = GetComponent<WorldGeneration>();
    }

    private void Start() {
        grid = worldGeneration.ReturnGrid();

        SpawnCloudsOnGrid();
    }

    private void SpawnCloudsOnGrid() {
        for (int x = -1; x < grid.gridArray.GetLength(0) + 2; x ++) {
            for (int y = -1; y < grid.gridArray.GetLength(1) + 2; y++) {
                for (int t = 1; t <= amountOfClouds; t++)
                {
                    Vector3 gridLocation = grid.GetWorldPosition(x, y) + new Vector3(worldGeneration.cellSize - 3f, worldGeneration.cellSize, -1f) * .5f + new Vector3(amountOfClouds / t, amountOfClouds / t);
                    int randomNum = Random.Range(0, cloudPrefabs.Length);
                    GameObject newCloud = Instantiate(cloudPrefabs[randomNum], gridLocation, transform.rotation);
                    newCloud.transform.SetParent(cloudParent);
                }
            }
        }
    }
}
