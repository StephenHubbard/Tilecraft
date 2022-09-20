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
        foreach (KeyValuePair<string, ItemInfo> kvp in data.draggableItemWorkers) {
            Vector3 workerPos;
            data.draggableItemWorkersPos.TryGetValue(kvp.Key, out workerPos);

            ItemInfo itemInfo;
            data.draggableItemWorkers.TryGetValue(kvp.Key, out itemInfo);
            GameObject loadedItem = Instantiate(itemInfo.draggableItemPrefab, workerPos, transform.rotation);
        }
    }

    public void SpawnPlacedWorkers(GameData data) {
        foreach (KeyValuePair<string, ItemInfo> kvp in data.placedItemWorkers) {
            Vector3 workerPos;
            data.placedItemsWorkersPos.TryGetValue(kvp.Key, out workerPos);

            ItemInfo itemInfo;
            data.placedItemWorkers.TryGetValue(kvp.Key, out itemInfo);
            GameObject loadedItem = Instantiate(itemInfo.onTilePrefab, workerPos, transform.rotation);

            RaycastHit2D[] hitArray = Physics2D.RaycastAll(loadedItem.transform.position, Vector2.zero, 100f);

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
                    loadedItem.transform.SetParent(parentTransform);
                    loadedItem.transform.position = parentTransform.position;
                    break;
                }
            }

            parentTile.GetComponent<CraftingManager>().hasWorkers = true;
            StartCoroutine(parentTile.CheckValideRecipeEndOfFrameCo());
        }
    }

    
}
