using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [SerializeField] public string toolTipName;

    [TextArea]
    [SerializeField] public string toolTipText;

    [SerializeField] public ItemInfo itemInfo;

    public GameObject shownItemsContainer;
    public bool isPackIcon = false;
    public bool isEncyclopediaIcon = false;

    private EconomyManager economyManager;
    private ToolTipManager toolTipManager;

    private void Awake() {
        economyManager = FindObjectOfType<EconomyManager>();
        toolTipManager = FindObjectOfType<ToolTipManager>();
        shownItemsContainer = GameObject.Find("ShownItemsContainer");
    }

    private void Start() {
        if (itemInfo) {
            toolTipName = itemInfo.itemName;
        }

        if (isEncyclopediaIcon) {
            toolTipText = "Required Components:";
        }
    }

    private void Update() {
        // if (toolTipName == "Coins") {
        //     toolTipText = "You currently have " + economyManager.currentCoins + " coins.";
        // }
    }

    public void UpdatePackUIToolTip() {

        if (!toolTipManager.isMaximized) { return; }

        if (GetComponent<PackManager>()) {
            shownItemsContainer.SetActive(true);

            foreach (var availableItem in GetComponent<PackManager>().resourcePackItems)
            {
                // could later on turn resourcePackItems into a dictionary and display unknown resources in the pack as question marks
                GameObject newImageObject = new GameObject("available item");
                newImageObject.transform.SetParent(shownItemsContainer.transform);
                newImageObject.transform.localScale = Vector3.one;
                newImageObject.AddComponent<Image>();
                newImageObject.GetComponent<Image>().sprite = availableItem.itemSprite;
            }
        }

        if (isEncyclopediaIcon) {
            if (shownItemsContainer == null) {
                shownItemsContainer = GameObject.Find("ShownItemsContainer");
            }

            shownItemsContainer.SetActive(true);

            foreach (var requiredResource in itemInfo.recipeInfo.neededRecipeItems)
            {
                GameObject newImageObject = new GameObject("required crafting item");
                newImageObject.transform.SetParent(shownItemsContainer.transform);
                newImageObject.transform.localScale = Vector3.one;
                newImageObject.AddComponent<Image>();
                newImageObject.GetComponent<Image>().sprite = requiredResource.itemSprite;
            }

            if (itemInfo.recipeInfo.neededRecipeItems.Length == 0) {
                toolTipText = "Required Components: Uncraftable";
            }
        }
    }

    public void ClearPackUIToolTip() {
        if (isPackIcon || isEncyclopediaIcon) {
            foreach (Transform child in shownItemsContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
