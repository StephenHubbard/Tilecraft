using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTiles : MonoBehaviour
{
    [SerializeField] private GameObject blankTilePrefab;

    public static LoadTiles instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnTiles(GameData data) {
        foreach (KeyValuePair<string, TileInfo> kvp in data.tileInfo) {
            
            Vector3 tilePos;
            data.tilePositions.TryGetValue(kvp.Key, out tilePos);
            GameObject newTile = Instantiate(blankTilePrefab, tilePos, transform.rotation);
            GameObject tileType = Instantiate(kvp.Value.tilePrefab, newTile.transform.position, transform.rotation);
            tileType.transform.SetParent(newTile.transform);
            Tile thisTile = newTile.GetComponent<Tile>();
            thisTile.tileInfo = kvp.Value;

            ItemInfo itemInfo;
            data.itemInfo.TryGetValue(kvp.Key, out itemInfo);
            thisTile.itemInfo = itemInfo;

            if (itemInfo != null) {
                GameObject currentPlacedItem = Instantiate(itemInfo.onTilePrefab, newTile.transform.position, transform.rotation);
                currentPlacedItem.transform.SetParent(newTile.transform);
                thisTile.currentPlacedItem = currentPlacedItem;
                thisTile.currentPlacedResources.Add(itemInfo);
                currentPlacedItem.GetComponent<PlacedItem>().CheckForValidRecipe();
            }
        }
    }

}
