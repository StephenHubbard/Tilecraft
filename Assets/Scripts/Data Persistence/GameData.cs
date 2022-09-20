using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Economy Manager
    public int currentXP;
    public int xpTillDiscovery;

    // Discovery Manager
    public int discoveryIndex;

    // Tiles 
    public SerializableDictionary<string, Vector3> tilePositions;
    public SerializableDictionary<string, TileInfo> tileInfo;
    public SerializableDictionary<string, ItemInfo> itemInfo;

    // Items
    public SerializableDictionary<string, Vector3> draggableItemPositions;
    public SerializableDictionary<string, ItemInfo> draggableItemsItemInfo;

    // Clouds
    public SerializableDictionary<string, Vector3> cloudPositions;

    // Placed Items
    public SerializableDictionary<string, ItemInfo> placedItems;
    public SerializableDictionary<string, Vector3> placedItemsPos;

    // Workers
    public SerializableDictionary<string, ItemInfo> draggableItemWorkers;
    public SerializableDictionary<string, Vector3> draggableItemWorkersPos;

    public SerializableDictionary<string, ItemInfo> placedItemWorkers;
    public SerializableDictionary<string, Vector3> placedItemsWorkersPos;

    // Storage
    public SerializableDictionary<string, ItemInfo> storageItemsItemInfo;
    public SerializableDictionary<string, int> storageItemsAmount;

    // Encyclopedia
    public SerializableDictionary<ItemInfo, bool> discoveredItemsCraftedDict;


    public GameData() {
        // Economy Manager
        this.currentXP = 0;
        this.xpTillDiscovery = 0;

        // Discovery Manager
        this.discoveryIndex = 0;

        // Tiles
        tilePositions = new SerializableDictionary<string, Vector3>();
        tileInfo = new SerializableDictionary<string, TileInfo>();
        itemInfo = new SerializableDictionary<string, ItemInfo>();

        // Items
        draggableItemPositions = new SerializableDictionary<string, Vector3>();
        draggableItemsItemInfo = new SerializableDictionary<string, ItemInfo>();

        // Clouds
        cloudPositions = new SerializableDictionary<string, Vector3>();

        // PlacedItems
        placedItems = new SerializableDictionary<string, ItemInfo>();
        placedItemsPos = new SerializableDictionary<string, Vector3>();

        // Workers 
        draggableItemWorkers = new SerializableDictionary<string, ItemInfo>();
        draggableItemWorkersPos = new SerializableDictionary<string, Vector3>();

        placedItemWorkers = new SerializableDictionary<string, ItemInfo>();
        placedItemsWorkersPos = new SerializableDictionary<string, Vector3>();

        // Storage
        storageItemsItemInfo = new SerializableDictionary<string, ItemInfo>();
        storageItemsAmount = new SerializableDictionary<string, int>();

        // Encyclopedia
        discoveredItemsCraftedDict = new SerializableDictionary<ItemInfo, bool>();
    }
}
