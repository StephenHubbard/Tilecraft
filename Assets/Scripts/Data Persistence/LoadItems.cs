using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadItems : MonoBehaviour
{
    public static LoadItems instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnItems(GameData data) {
        foreach (KeyValuePair<string, ItemInfo> kvp in data.draggableItemsItemInfo) {
            Vector3 itemPos;
            data.draggableItemPositions.TryGetValue(kvp.Key, out itemPos);

            ItemInfo itemInfo;
            data.draggableItemsItemInfo.TryGetValue(kvp.Key, out itemInfo);
            GameObject loadedItem = Instantiate(itemInfo.draggableItemPrefab, itemPos, transform.rotation);
        }
    }

    public void SpawnPlacedItems(GameData data) {
        foreach (KeyValuePair<string, ItemInfo> kvp in data.placedItems)
        {
            Vector3 itemPos;
            data.placedItemsPos.TryGetValue(kvp.Key, out itemPos);

            ItemInfo itemInfo;
            data.placedItems.TryGetValue(kvp.Key, out itemInfo);
            GameObject loadedItem = Instantiate(itemInfo.onTilePrefab, itemPos, transform.rotation);

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

            foreach (var resourcePoint in parentTile.resourcePoints)
            {
                if (resourcePoint.transform.childCount == 0) {
                    parentTransform = resourcePoint;
                    loadedItem.transform.SetParent(parentTransform);
                    loadedItem.transform.position = parentTransform.position;
                    break;
                }
            }
        }

        CurrentPlacedResourcesOnTileList();
    }


    public void CurrentPlacedResourcesOnTileList() {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (var tile in allTiles)
        {
            foreach (var resourcePoint in tile.resourcePoints)
            {
                if (resourcePoint.childCount > 0) {
                    tile.currentPlacedResources.Add(resourcePoint.GetChild(0).GetComponent<PlacedItem>().itemInfo);
                }
            }
        }
    }
}
