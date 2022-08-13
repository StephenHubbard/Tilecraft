using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoveryManager : MonoBehaviour
{
    public static DiscoveryManager instance { get; set;}

    [SerializeField] public List<ItemInfo> allAvailableItems = new List<ItemInfo>();

    [SerializeField] public List<ItemInfo> allTierOneItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierTwoItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierThreeItems = new List<ItemInfo>();

    [SerializeField] public List<ItemInfo> allKnownItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierOneItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierTwoItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierThreeItems = new List<ItemInfo>();

    private void Awake() {
        instance = this;
    }

    private void Start() {
        SortAllItems();
        FindAllDiscoveredItems();
    }

    private void SortAllItems() {
        foreach (var item in allAvailableItems)
        {
            if (item.tierGroup == ItemInfo.TierGroup.one) {
                allTierOneItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.two) {
                allTierTwoItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.three) {
                allTierThreeItems.Add(item);
            }
        }
    }

    private void FindAllDiscoveredItems() {
        foreach (var item in Encyclopedia.instance.discoveredItems)
        {
            if (item.tierGroup == ItemInfo.TierGroup.one) {
                knownTierOneItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.two) {
                knownTierTwoItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.three) {
                knownTierThreeItems.Add(item);
            }
            allKnownItems.Add(item);
        }
    }

    public void NewDiscoveredItem(ItemInfo item) {
        if (item.tierGroup == ItemInfo.TierGroup.one) {
                knownTierOneItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.two) {
                knownTierTwoItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.three) {
                knownTierThreeItems.Add(item);
            }
            allKnownItems.Add(item);
    }

    public void DetermineNewDiscovery() {
        ShuffleList(allTierOneItems);
        foreach (var item in allTierOneItems)
        {
            if (!knownTierOneItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item);
                return;
            } 
        }

        ShuffleList(allTierTwoItems);
        foreach (var item in allTierTwoItems)
        {
            if (!knownTierTwoItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item);
                return;
            } 
        }

        ShuffleList(allTierThreeItems);
        foreach (var item in allTierThreeItems)
        {
            if (!knownTierThreeItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item);
                return;
            } 
        }
    }

    private void ShuffleList<T>(List<T> inputList) {
        for (int i = 0; i < inputList.Count - 1; i++)
        {
            T temp = inputList[i];
            int rand = Random.Range(i, inputList.Count);
            inputList[i] = inputList[rand];
            inputList[rand] = temp;
        }
    }
}
