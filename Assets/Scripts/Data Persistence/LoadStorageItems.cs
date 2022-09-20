using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStorageItems : MonoBehaviour
{


    public static LoadStorageItems instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnStorageItems(GameData data) {
        foreach (KeyValuePair<string, ItemInfo> kvp in data.storageItemsItemInfo) {
            ItemInfo itemInfo;
            data.storageItemsItemInfo.TryGetValue(kvp.Key, out itemInfo);

            int amount;
            data.storageItemsAmount.TryGetValue(kvp.Key, out amount);

            StorageContainer.instance.AddToStorage(itemInfo, amount);
        }
    }
}
