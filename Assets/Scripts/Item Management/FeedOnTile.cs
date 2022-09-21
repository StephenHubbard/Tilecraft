using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedOnTile : MonoBehaviour
{
    private CraftingManager craftingManager;

    private void Awake() {
        craftingManager = GetComponent<CraftingManager>();
    }

    public bool CheckIfCanEat() {
        bool isFoodOnly = false;

        foreach (var resourcePoint in GetComponent<Tile>().resourcePoints)
        {
            if (resourcePoint.childCount > 0) {
                if (resourcePoint.GetChild(0).GetComponent<Food>()) {
                    isFoodOnly = true;
                } else {
                    isFoodOnly = false;
                    break;
                }
            }
        }

        if (isFoodOnly) {
            return true;
        } else {
            return false;
        }
    }

    public void FeedPopulation(Population population) {
        foreach (var resourcePoint in GetComponent<Tile>().resourcePoints)
        {
            if (resourcePoint.childCount > 0) {
                if (resourcePoint.GetChild(0).GetComponent<Food>()) {
                    Destroy(resourcePoint.GetChild(0));
                    break;
                }
            }
        }

        foreach (var workerPoint in GetComponent<Tile>().workerPoints)
        {
            if (workerPoint.childCount > 0) {
                if (workerPoint.GetChild(0).GetComponent<Worker>()) {
                    workerPoint.GetChild(0).GetComponent<Worker>().FeedWorker(1, true);
                    break;
                }
                if (workerPoint.GetChild(0).GetComponent<Archer>()) {
                    workerPoint.GetChild(0).GetComponent<Archer>().FeedArcher(1, true);
                    break;
                }
                if (workerPoint.GetChild(0).GetComponent<Knight>()) {
                    workerPoint.GetChild(0).GetComponent<Knight>().FeedKnight(1, true);
                    break;
                }
            }
        }
    }
}
