using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscoveryManager : MonoBehaviour
{
    public bool discoverAllItems = false;

    public static DiscoveryManager instance { get; set;}

    [SerializeField] public List<ItemInfo> allAvailableItems = new List<ItemInfo>();

    [SerializeField] public List<ItemInfo> allTierOneItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierTwoItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierThreeItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierFourItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierFiveItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> allTierSixItems = new List<ItemInfo>();

    [SerializeField] public List<ItemInfo> allKnownItems = new List<ItemInfo>();

    [SerializeField] public List<ItemInfo> knownTierOneItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierTwoItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierThreeItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierFourItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierFiveItems = new List<ItemInfo>();
    [SerializeField] public List<ItemInfo> knownTierSixItems = new List<ItemInfo>();

    [SerializeField] private GameObject newDiscoveryAnimPrefab;
    [SerializeField] private Transform canvasUI;

    

    private void Awake() {
        instance = this;
    }

    private void Start() {
        SortAllItems();
        FindAllDiscoveredItems();

        if (discoverAllItems) {
            DiscoverAllItems();
        }
    }

    // debug use only
    private void DiscoverAllItems() {
        for (int i = 0; i < allAvailableItems.Count; i++)
        {
            Encyclopedia.instance.AddItemToDiscoveredList(allAvailableItems[i], false);
        }
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
            if (item.tierGroup == ItemInfo.TierGroup.four) {
                allTierFourItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.five) {
                allTierFiveItems.Add(item);
            }
            if (item.tierGroup == ItemInfo.TierGroup.six) {
                allTierSixItems.Add(item);
            }
        }
    }

    private void FindAllDiscoveredItems() {
        foreach (var item in Encyclopedia.instance.discoveredItems)
        {
            if (item.Key.tierGroup == ItemInfo.TierGroup.one) {
                knownTierOneItems.Add(item.Key);
            }
            if (item.Key.tierGroup == ItemInfo.TierGroup.two) {
                knownTierTwoItems.Add(item.Key);
            }
            if (item.Key.tierGroup == ItemInfo.TierGroup.three) {
                knownTierThreeItems.Add(item.Key);
            }
            if (item.Key.tierGroup == ItemInfo.TierGroup.four) {
                knownTierFourItems.Add(item.Key);
            }
            if (item.Key.tierGroup == ItemInfo.TierGroup.five) {
                knownTierFiveItems.Add(item.Key);
            }
            if (item.Key.tierGroup == ItemInfo.TierGroup.six) {
                knownTierSixItems.Add(item.Key);
            }
            allKnownItems.Add(item.Key);
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
        if (item.tierGroup == ItemInfo.TierGroup.four) {
            knownTierFourItems.Add(item);
        }
        if (item.tierGroup == ItemInfo.TierGroup.five) {
            knownTierFiveItems.Add(item);
        }
        if (item.tierGroup == ItemInfo.TierGroup.six) {
            knownTierSixItems.Add(item);
        }
        allKnownItems.Add(item);
    }

    public void DetermineNewDiscovery() {
        ShuffleList(allTierOneItems);
        foreach (var item in allTierOneItems)
        {
            if (!knownTierOneItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item, true);
                NewDiscoveryAnimation(item);
                return;
            } 
        }

        ShuffleList(allTierTwoItems);
        foreach (var item in allTierTwoItems)
        {
            if (!knownTierTwoItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item, true);
                NewDiscoveryAnimation(item);
                return;
            } 
        }

        ShuffleList(allTierThreeItems);
        foreach (var item in allTierThreeItems)
        {
            if (!knownTierThreeItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item, true);
                NewDiscoveryAnimation(item);
                return;
            } 
        }

        ShuffleList(allTierFourItems);
        foreach (var item in allTierFourItems)
        {
            if (!knownTierFourItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item, true);
                NewDiscoveryAnimation(item);
                return;
            } 
        }

        ShuffleList(allTierFiveItems);
        foreach (var item in allTierFiveItems)
        {
            if (!knownTierFiveItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item, true);
                NewDiscoveryAnimation(item);
                return;
            } 
        }

        ShuffleList(allTierSixItems);
        foreach (var item in allTierSixItems)
        {
            if (!knownTierSixItems.Contains(item)) {
                Encyclopedia.instance.AddItemToDiscoveredList(item, true);
                NewDiscoveryAnimation(item);
                return;
            } 
        }

        EconomyManager.instance.AllItemsDiscovered();
    }

    

    public void NewDiscoveryAnimation(ItemInfo itemInfo) {
        GameObject newAnimPrefab = Instantiate(newDiscoveryAnimPrefab, canvasUI.transform.position, transform.rotation);
        newAnimPrefab.transform.SetParent(canvasUI.transform.parent);
        newAnimPrefab.transform.localScale = Vector3.one;

        GameObject itemSprite = new GameObject("itemSprite");
        itemSprite.AddComponent<Image>();
        itemSprite.GetComponent<Image>().sprite = itemInfo.itemSprite;
        itemSprite.transform.SetParent(newAnimPrefab.transform);
        itemSprite.transform.localScale = new Vector3(.5f, .5f, .5f);
        itemSprite.transform.position = newAnimPrefab.transform.position;
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
