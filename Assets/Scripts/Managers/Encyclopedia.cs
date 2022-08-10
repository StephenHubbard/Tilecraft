using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Encyclopedia : MonoBehaviour
{
    [SerializeField] private Image newDiscoveredItemPrefab;
    [SerializeField] private GameObject encylopediaContainer;
    [SerializeField] private GameObject encylopediaGridLayout;
    [SerializeField] private List<ItemInfo> discoveredItems = new List<ItemInfo>();

    private ToolTipManager toolTipManager;
    private GameObject shownItemsContainer;

    public static Encyclopedia instance;

    private void Awake() {
        toolTipManager = FindObjectOfType<ToolTipManager>();

        shownItemsContainer = GameObject.Find("ShownItemsContainer");

        if (instance == null) {
            instance = this;
        }
    }

    public void OpenEncylopedia() {
        if (encylopediaContainer.activeInHierarchy) {
            encylopediaContainer.SetActive(false);
        } else {
            encylopediaContainer.SetActive(true);
        }
    }

    public void CloseEncylopedia() {
        encylopediaContainer.SetActive(false);
    }

    public void AddItemToDiscoveredList(ItemInfo newItem) {
        if (!discoveredItems.Contains(newItem)) {
            discoveredItems.Add(newItem);
            NewItemToEncyclopedia(newItem);
        } 
    }

    private void NewItemToEncyclopedia(ItemInfo newItem) {
        GameObject discoveredItem = Instantiate(newDiscoveredItemPrefab.gameObject, transform.position, transform.rotation);
        discoveredItem.transform.SetParent(encylopediaGridLayout.transform);
        discoveredItem.transform.localScale = Vector3.one;
        discoveredItem.GetComponent<Image>().sprite = newItem.itemSprite;
        discoveredItem.GetComponent<UITooltip>().itemInfo = newItem;

        EventTrigger thisHoverTrigger = discoveredItem.GetComponent<EventTrigger>();
        EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => { OnPointerHoverDelegate(discoveredItem.transform); });
        thisHoverTrigger.triggers.Add(hoverEntry);
        
        EventTrigger thisExitTrigger = discoveredItem.GetComponent<EventTrigger>();
        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { OnPointerExitDelegate(discoveredItem.transform); });
        thisHoverTrigger.triggers.Add(exitEntry);
    }

    public void OnPointerHoverDelegate(Transform discoveredItem)
    {
        toolTipManager.HoverOverUI(discoveredItem);
    }

    public void OnPointerExitDelegate(Transform discoveredItem)
    {
        foreach (Transform child in shownItemsContainer.transform)
            {
                Destroy(child.gameObject);
            }
    }
}
