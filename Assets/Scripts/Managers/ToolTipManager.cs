using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class ToolTipManager : MonoBehaviour
{
    [SerializeField] private TMP_Text itemText; 
    [SerializeField] private TMP_Text toolTipText; 
    [SerializeField] private GameObject appleContainer;
    [SerializeField] private GameObject pickaxeContainer;
    [SerializeField] private GameObject coinContainer;
    [SerializeField] private GameObject heartContainer;
    [SerializeField] private GameObject availablePackItemsContainer;
    [SerializeField] private TMP_Text foodValueText;
    [SerializeField] private TMP_Text coinValueText;
    [SerializeField] private TMP_Text heartValueText;
    [SerializeField] private TMP_Text pickaxeValueText;
    [SerializeField] private GameObject toolTipImage;
    [SerializeField] private GameObject maximizeButton;
    [SerializeField] private GameObject minimizeButton;
    [SerializeField] private LayerMask toolTipLayerMask = new LayerMask();

    public bool isMaximized = true;
    public bool isOverUI = false;

    public static ToolTipManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        ToggleToolTipOff();
        // MinimizeWindow();
    }

    private void Update() {
        RaycastMouseOver();
    }

    private void RaycastMouseOver() {
        if (!isMaximized || isOverUI) { return; }

        RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, toolTipLayerMask);

        if (hit.Length > 0) {
        
            ToggleToolTipOn();
            if (hit.Length > 1 && hit[0].transform.GetComponent<Food>() && hit[1].transform.GetComponent<Worker>()) {
                string newStr = "You are about to feed worker " + (hit[0].transform.GetComponent<Food>().foodWorthAmount * hit[0].transform.GetComponent<Stackable>().amountOfChildItems).ToString() + " food";
                UpdateValues("Feed Worker?", newStr, 0, 0, 0, 0);
                return;
            }

            else if (hit[0].transform.GetComponent<DraggableItem>()) {
                ItemInfo thisItem = hit[0].transform.GetComponent<DraggableItem>().itemInfo;
                if (hit[0].transform.GetComponent<Worker>()) {
                    int healthValue = hit[0].transform.GetComponent<Worker>().myHealth;
                    int workStrengthValue = hit[0].transform.GetComponent<Worker>().myStrength;
                    string neededFoodToUpgradeStrength = "Food until power up: " + hit[0].transform.GetComponent<Worker>().foodNeededToUpPickaxeStrengthCurrent.ToString();
                    UpdateValues(thisItem.itemName, neededFoodToUpgradeStrength, thisItem.foodValue, thisItem.coinValue, healthValue, workStrengthValue);
                    return;
                } else {
                    UpdateValues(thisItem.itemName, thisItem.toolTipText, thisItem.foodValue, thisItem.coinValue, 0, 0);
                    return;
                }
            } 
            
            else if (hit[0].transform.GetComponent<Tile>()) {
                TileInfo thisTile = hit[0].transform.GetComponent<Tile>().tileInfo;
                if (hit[0].transform.GetComponent<Tile>().currentPlacedItem != null) {
                    string newStr = hit[0].transform.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.itemName + ": " + hit[0].transform.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.toolTipText;
                    UpdateValues(thisTile.name, newStr, 0, 0, 0, 0);
                    return;
                } else {
                    UpdateValues(thisTile.name, null, 0, 0, 0, 0);
                    return;
                }
            } 
        } else {
            ToggleToolTipOff();
        }
    }

    public void ToggleToolTipOn() {
        itemText.gameObject.SetActive(true);
        toolTipText.gameObject.SetActive(true);
        appleContainer.gameObject.SetActive(true);
        coinContainer.gameObject.SetActive(true);
        heartContainer.gameObject.SetActive(true);
        pickaxeContainer.gameObject.SetActive(true);
    }

    public void ToggleToolTipOff() {
        itemText.gameObject.SetActive(false);
        toolTipText.gameObject.SetActive(false);
        appleContainer.gameObject.SetActive(false);
        coinContainer.gameObject.SetActive(false);
        heartContainer.gameObject.SetActive(false);
        pickaxeContainer.gameObject.SetActive(false);
    }

    public void UpdateValues(string itemText, string toolTipText, int foodValue, int coinValue, int heartValue, int pickaxeValue) {

        this.itemText.text = itemText;
        this.toolTipText.text = toolTipText;
        if (foodValue > 0) {
            this.foodValueText.text = foodValue.ToString();
        } else {
            appleContainer.SetActive(false);
        }
        if (coinValue > 0) {
            this.coinValueText.text = coinValue.ToString();
        } else {
            coinContainer.SetActive(false);
        }
        if (heartValue > 0) {
            this.heartValueText.text = heartValue.ToString();
        } else {
            heartContainer.SetActive(false);
        }
        if (pickaxeValue > 0) {
            this.pickaxeValueText.text = pickaxeValue.ToString();
        } else {
            pickaxeContainer.SetActive(false);
        }
    }

    public void MinimizeWindow() {
        ToggleToolTipOff();
        toolTipImage.SetActive(false);
        maximizeButton.SetActive(true);
        minimizeButton.SetActive(false);
        isMaximized = false;
        AudioManager.instance.Play("UI Click");
    }

    public void MaximizeWindow() {
        ToggleToolTipOn();
        toolTipImage.SetActive(true);
        maximizeButton.SetActive(false);
        minimizeButton.SetActive(true);
        isMaximized = true;
        AudioManager.instance.Play("UI Click");
    }

    // event listener in inspector
    public void HoverOverUI(Transform sender) {
        if (!isMaximized) { return; }

        isOverUI = true;
        ToggleToolTipOn();
        if (sender.GetComponent<UITooltip>().isPackIcon || sender.GetComponent<UITooltip>().isEncyclopediaIcon) {
            sender.GetComponent<UITooltip>().UpdatePackUIToolTip();
        }
        UpdateValues(sender.GetComponent<UITooltip>().toolTipName, sender.GetComponent<UITooltip>().toolTipText, 0, 0, 0, 0);
    }

    // event listener in inspector
    public void ExitUI(Transform sender) {
        if (!isMaximized) { return; }

        isOverUI = false;
        ToggleToolTipOff();
        if (sender.GetComponent<UITooltip>().isPackIcon || sender.GetComponent<UITooltip>().isEncyclopediaIcon) {
            sender.GetComponent<UITooltip>().ClearPackUIToolTip();
            // sender.GetComponent<UITooltip>().shownItemsContainer.SetActive(false);
        }
    }

}
