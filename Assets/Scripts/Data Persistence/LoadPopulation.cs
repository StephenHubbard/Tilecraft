using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPopulation : MonoBehaviour
{
    public static LoadPopulation instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnDraggableItemWorkers(GameData data) {
        foreach (KeyValuePair<string, ItemInfo> kvp in data.draggableItemPopulation) {
            Vector3 popPos;
            data.draggablePopulationPos.TryGetValue(kvp.Key, out popPos);

            ItemInfo itemInfo;
            data.draggableItemPopulation.TryGetValue(kvp.Key, out itemInfo);
            GameObject loadedWorker = Instantiate(itemInfo.draggableItemPrefab, popPos, transform.rotation);

            int currentLevel;
            data.populationLevels.TryGetValue(kvp.Key, out currentLevel);
            for (int i = 0; i < currentLevel; i++)
            {
                if (loadedWorker.GetComponent<Worker>()) {
                    loadedWorker.GetComponent<Worker>().LevelUpStrength(0, false);
                }

                if (loadedWorker.GetComponent<Archer>()) {
                    loadedWorker.GetComponent<Archer>().LevelUpStrength(0, false);
                }

                if (loadedWorker.GetComponent<Knight>()) {
                    loadedWorker.GetComponent<Knight>().LevelUpStrength(0, false);
                }
            }
        }
    }

    public void SpawnPlacedWorkers(GameData data) {
        foreach (KeyValuePair<string, ItemInfo> kvp in data.placedItemPopulation) {
            Vector3 popPos;
            data.placedItemsPopulationPos.TryGetValue(kvp.Key, out popPos);

            ItemInfo itemInfo;
            data.placedItemPopulation.TryGetValue(kvp.Key, out itemInfo);
            GameObject loadedWorker = Instantiate(itemInfo.onTilePrefab, popPos, transform.rotation);

            int currentLevel;
            data.populationLevels.TryGetValue(kvp.Key, out currentLevel);
            for (int i = 0; i < currentLevel; i++)
            {
                if (loadedWorker.GetComponent<Worker>()) {
                    loadedWorker.GetComponent<Worker>().LevelUpStrength(0, false);
                }

                if (loadedWorker.GetComponent<Archer>()) {
                    loadedWorker.GetComponent<Archer>().LevelUpStrength(0, false);
                }

                if (loadedWorker.GetComponent<Knight>()) {
                    loadedWorker.GetComponent<Knight>().LevelUpStrength(0, false);
                }
            }

            RaycastHit2D[] hitArray = Physics2D.RaycastAll(loadedWorker.transform.position, Vector2.zero, 100f);

            Tile parentTile = null;
            Transform parentTransform = null;

            foreach (var item in hitArray)
            {
                if (item.transform.GetComponent<Tile>()) {
                    parentTile = item.transform.GetComponent<Tile>();
                    break;
                }
            }

            foreach (var workerPoint in parentTile.workerPoints)
            {
                if (workerPoint.transform.childCount == 0) {
                    parentTransform = workerPoint;
                    loadedWorker.transform.SetParent(parentTransform);
                    loadedWorker.transform.position = parentTransform.position;
                    break;
                }
            }

            parentTile.GetComponent<CraftingManager>().hasWorkers = true;
            StartCoroutine(parentTile.CheckValideRecipeEndOfFrameCo());
        }
    }

}
