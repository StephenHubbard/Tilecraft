using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UITooltip : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public string toolTipName;

    [TextArea]
    [SerializeField] public string toolTipText;

    [SerializeField] public ItemInfo itemInfo;

    [HideInInspector]
    public GameObject shownItemsContainer;
    public bool isEncyclopediaIcon = false;

    private EconomyManager economyManager;
    private ToolTipManager toolTipManager;

    [SerializeField] private GameObject toDoListActiveBorderPrefab;


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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && itemInfo != null) {
            Encyclopedia.instance.DisplayWhatRecipesCanBeMade(transform);
            AudioManager.instance.Play("UI Click");
        }

        if (eventData.button == PointerEventData.InputButton.Right && itemInfo != null) { 

            if (itemInfo && !IsItemAlreadyInToDoList() && ToDoManager.instance.toDoList.Count < 4) {
                ToDoManager.instance.SetNewToDoList(itemInfo);
                GameObject newBorder = Instantiate(toDoListActiveBorderPrefab, transform.position, transform.rotation);
                newBorder.transform.SetParent(this.transform);
                newBorder.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
            } else {
                foreach (var item in ToDoManager.instance.toDoList)
                {
                    if (item.GetComponent<ToDoList>().itemInfo == itemInfo) {
                        ToDoManager.instance.toDoList.Remove(item.gameObject);
                        Destroy(item.gameObject);
                        Destroy(transform.GetChild(0).gameObject);
                        break;
                    }
                }
            }

            AudioManager.instance.Play("UI Click");
        }
    }

    public void ItemCraftedTakeOffToDoList(ItemInfo itemInfo) {
        foreach (var item in ToDoManager.instance.toDoList)
        {
            if (item.GetComponent<ToDoList>().itemInfo == itemInfo) {
                ToDoManager.instance.toDoList.Remove(item.gameObject);
                Destroy(item.gameObject);
                Destroy(transform.GetChild(0).gameObject);
                break;
            }
        }
    }

    public void AddAutoToDoList() {
        if (itemInfo && !IsItemAlreadyInToDoList() && ToDoManager.instance.toDoList.Count < 4) {
            ToDoManager.instance.SetNewToDoList(itemInfo);
            GameObject newBorder = Instantiate(toDoListActiveBorderPrefab, transform.position, transform.rotation);
            newBorder.transform.SetParent(this.transform);
            newBorder.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
        } else {
            foreach (var item in ToDoManager.instance.toDoList)
            {
                if (item.GetComponent<ToDoList>().itemInfo == itemInfo) {
                    ToDoManager.instance.toDoList.Remove(item.gameObject);
                    Destroy(item.gameObject);
                    Destroy(transform.GetChild(0).gameObject);
                    break;
                }
            }
        }
        
    }

    private bool IsItemAlreadyInToDoList() {
        foreach (var item in ToDoManager.instance.toDoList)
        {
            if (item.GetComponent<ToDoList>().itemInfo == itemInfo) {
                return true;
            }
        }

        return false;
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

    
}
