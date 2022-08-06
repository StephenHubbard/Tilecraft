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
    [SerializeField] private GameObject coinContainer;
    [SerializeField] private TMP_Text foodValueText;
    [SerializeField] private TMP_Text coinValueText;
    [SerializeField] private GameObject toolTipImage;
    [SerializeField] private GameObject maximizeButton;
    [SerializeField] private GameObject minimizeButton;
    [SerializeField] private LayerMask toolTipLayerMask = new LayerMask();

    private bool isMaximized = true;

    private void Start() {
        ToggleToolTipOff();
        // MinimizeWindow();
    }

    private void Update() {
        RaycastMouseOver();
    }

    private void RaycastMouseOver() {
        if (!isMaximized) { return; }

        RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, toolTipLayerMask);

        if (hit.Length > 0) {
            ToggleToolTipOn();
            if (hit[0].transform.GetComponent<DraggableItem>()) {
                ItemInfo thisItem = hit[0].transform.GetComponent<DraggableItem>().itemInfo;
                UpdateValues(thisItem.itemName, thisItem.toolTipText, thisItem.foodValue, thisItem.coinValue);
            } else if (hit[0].transform.GetComponent<Tile>()) {
                TileInfo thisTile = hit[0].transform.GetComponent<Tile>().tileInfo;
                if (hit[0].transform.GetComponent<Tile>().currentPlacedItem != null) {
                    string newStr = hit[0].transform.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.itemName + ": " + hit[0].transform.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.toolTipText;
                    UpdateValues(thisTile.name, newStr, 0, 0);
                } else {
                    UpdateValues(thisTile.name, null, 0, 0);
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
    }

    public void ToggleToolTipOff() {
        itemText.gameObject.SetActive(false);
        toolTipText.gameObject.SetActive(false);
        appleContainer.gameObject.SetActive(false);
        coinContainer.gameObject.SetActive(false);
    }

    public void UpdateValues(string itemText, string toolTipText, int foodValue, int coinValue) {
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

}
