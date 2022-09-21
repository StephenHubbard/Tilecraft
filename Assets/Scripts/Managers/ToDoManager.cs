using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToDoManager : MonoBehaviour
{
    [SerializeField] private GameObject toDoContainer;
    [SerializeField] private GameObject maximizeButton;
    [SerializeField] private GameObject toDoListPrefab;
    [SerializeField] private GameObject listContainerGridLayoutParent;
    [SerializeField] private Sprite notCraftableSprite;
    [SerializeField] private Sprite plusIconSprite;
    [SerializeField] private Sprite furnaceSprite;
    [SerializeField] private Sprite alterSprite;

    [SerializeField] public List<GameObject> toDoList = new List<GameObject>();


    public static ToDoManager instance;

    private void Awake() {
        instance = this;
    }


    public void ToggleMinMaxWindow() {
        if (toDoContainer.activeInHierarchy == true) {
            toDoContainer.SetActive(false);
            maximizeButton.SetActive(true);
        } else {
            toDoContainer.SetActive(true);
            maximizeButton.SetActive(false);
        }

        AudioManager.instance.Play("UI Click");

    }

    public void CraftedItemTakeOffToDoList(ItemInfo itemInfo) {
        foreach (Transform item in Encyclopedia.instance.encylopediaGridLayout.transform)
        {
            if (item.GetComponent<UITooltip>().itemInfo == itemInfo) {
                item.GetComponent<UITooltip>().ItemCraftedTakeOffToDoList(itemInfo);
            }
        }
    }

    public void FillNextToDoListItem() {
        int amountOfIterations = 4 - toDoList.Count;

        for (int i = 0; i < amountOfIterations; i++)
        {
            foreach (Transform item in Encyclopedia.instance.encylopediaGridLayout.transform)
            {
                if (Encyclopedia.instance.discoveredItems[item.GetComponent<UITooltip>().itemInfo] == false && item.GetComponent<UITooltip>().itemInfo.recipeInfo.neededRecipeItems.Length > 0) {
                    item.GetComponent<UITooltip>().AddNextToDoListItemFromUncraftedList();
                }
            }
        }
    }

    public void SetNewToDoList(ItemInfo itemInfo) {
        GameObject newList = Instantiate(toDoListPrefab, toDoContainer.transform.position, transform.rotation);
        newList.transform.SetParent(listContainerGridLayoutParent.transform);
        newList.transform.localScale = Vector3.one;

        newList.transform.GetChild(0).GetComponent<Image>().sprite = itemInfo.itemSprite;
        newList.transform.GetChild(0).GetComponent<UITooltip>().itemInfo = itemInfo;
        newList.transform.GetChild(0).GetComponent<UITooltip>().toolTipText = itemInfo.toolTipText;

        // not craftable
        if (itemInfo.recipeInfo.neededRecipeItems.Length == 0) {
            newList.transform.GetChild(2).GetComponent<Image>().enabled = true;
            newList.transform.GetChild(2).GetComponent<Image>().sprite = notCraftableSprite;
        } else {
            newList.transform.GetChild(2).GetComponent<Image>().enabled = false;
        }
        
        if (itemInfo.recipeInfo.neededRecipeItems.Length > 0) {
            newList.transform.GetChild(2).GetComponent<Image>().enabled = true;
            newList.transform.GetChild(2).GetComponent<UITooltip>().itemInfo = itemInfo.recipeInfo.neededRecipeItems[0];
            newList.transform.GetChild(2).GetComponent<UITooltip>().toolTipText = itemInfo.recipeInfo.neededRecipeItems[0].toolTipText;
            newList.transform.GetChild(2).GetComponent<Image>().sprite = itemInfo.recipeInfo.neededRecipeItems[0].itemSprite;
        } 

        if (itemInfo.recipeInfo.neededRecipeItems.Length > 1) {
            newList.transform.GetChild(2).GetComponent<Image>().enabled = true;
            newList.transform.GetChild(3).GetComponent<UITooltip>().itemInfo = itemInfo.recipeInfo.neededRecipeItems[1];
            newList.transform.GetChild(3).GetComponent<UITooltip>().toolTipText = itemInfo.recipeInfo.neededRecipeItems[1].toolTipText;
            newList.transform.GetChild(3).GetComponent<Image>().sprite = itemInfo.recipeInfo.neededRecipeItems[1].itemSprite;
        } else {
            newList.transform.GetChild(3).GetComponent<Image>().enabled = false;
        }

        if (itemInfo.recipeInfo.neededRecipeItems.Length > 2) {
            newList.transform.GetChild(2).GetComponent<Image>().enabled = true;
            newList.transform.GetChild(4).GetComponent<UITooltip>().toolTipText = itemInfo.recipeInfo.neededRecipeItems[2].toolTipText;
            newList.transform.GetChild(4).GetComponent<UITooltip>().itemInfo = itemInfo.recipeInfo.neededRecipeItems[2];
            newList.transform.GetChild(4).GetComponent<Image>().sprite = itemInfo.recipeInfo.neededRecipeItems[2].itemSprite;
        } else {
            newList.transform.GetChild(4).GetComponent<Image>().enabled = false;
        }

        if (newList.transform.GetChild(0).GetComponent<UITooltip>().itemInfo.recipeInfo.requiresFurnace) {
                newList.transform.GetChild(3).GetComponent<Image>().enabled = true;
                newList.transform.GetChild(3).GetComponent<Image>().sprite = plusIconSprite;

                newList.transform.GetChild(4).GetComponent<Image>().enabled = true;

                if (newList.transform.GetChild(0).GetComponent<UITooltip>().itemInfo.itemName == "Ash") {
                    newList.transform.GetChild(4).GetComponent<Image>().sprite = alterSprite;
                } else {
                    newList.transform.GetChild(4).GetComponent<Image>().sprite = furnaceSprite;
                }
            }

        newList.GetComponent<ToDoList>().itemInfo = itemInfo;
        toDoList.Add(newList);
    }
}
