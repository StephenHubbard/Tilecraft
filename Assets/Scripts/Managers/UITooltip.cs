using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    [SerializeField] public string toolTipName;

    [TextArea]
    [SerializeField] public string toolTipText;

    [SerializeField] public GameObject packItemsContainer = null;

    private EconomyManager economyManager;

    private void Awake() {
        economyManager = FindObjectOfType<EconomyManager>();
    }

    private void Update() {
        if (toolTipName == "Coins") {
            toolTipText = "You currently have " + economyManager.currentCoins + " coins.";
        }
    }

    public void UpdatePackUIToolTip() {
        if (GetComponent<PackManager>()) {
            packItemsContainer.SetActive(true);

            foreach (var availableItem in GetComponent<PackManager>().resourcePackItems)
            {
                // could later on turn resourcePackItems into a dictionary and display unknown resources in the pack as question marks
                GameObject newImageObject = new GameObject("available item");
                newImageObject.transform.SetParent(packItemsContainer.transform);
                newImageObject.transform.localScale = Vector3.one;
                newImageObject.AddComponent<Image>();
                newImageObject.GetComponent<Image>().sprite = availableItem.itemSprite;
            }

        }
    }

    public void ClearPackUIToolTip() {
        if (GetComponent<PackManager>()) {
            foreach (Transform child in packItemsContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
