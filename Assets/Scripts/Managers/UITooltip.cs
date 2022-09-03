using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITooltip : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] public string toolTipName;

    [TextArea]
    [SerializeField] public string toolTipText;

    [SerializeField] public ItemInfo itemInfo;

    public GameObject shownItemsContainer;
    public bool isEncyclopediaIcon = false;
    public bool isStorageIcon = false;

    private EconomyManager economyManager;

    [SerializeField] private GameObject toDoListActiveBorderPrefab;

    private void Awake() {
        economyManager = FindObjectOfType<EconomyManager>();
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
        if (shownItemsContainer == null) {
            shownItemsContainer = GameObject.Find("ShownItemsContainer");
        }
        
        if (shownItemsContainer.activeInHierarchy) {
            toolTipText = "Required Components:";
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEncyclopediaIcon) { return; }

        if (eventData.button == PointerEventData.InputButton.Left && itemInfo != null && !isStorageIcon) {
            Encyclopedia.instance.DisplayWhatRecipesCanBeMade(transform);
            AudioManager.instance.Play("UI Click");
        }

        if (isEncyclopediaIcon && !isStorageIcon && eventData.button == PointerEventData.InputButton.Right && itemInfo != null  && transform.parent.name != "Potential Items Container Grid Layout") { 
        
            if (itemInfo && !IsItemAlreadyInToDoList() && ToDoManager.instance.toDoList.Count < 4) {
                ToDoManager.instance.SetNewToDoList(itemInfo);
                if (toDoListActiveBorderPrefab != null) {
                    GameObject newBorder = Instantiate(toDoListActiveBorderPrefab, transform.position, transform.rotation);
                    newBorder.transform.SetParent(this.transform);
                    newBorder.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                }
            } else {
                foreach (var item in ToDoManager.instance.toDoList)
                {
                    if (item.GetComponent<ToDoList>().itemInfo == itemInfo) {
                        ToDoManager.instance.toDoList.Remove(item.gameObject);
                        Destroy(item.gameObject);
                        if (transform.childCount > 0) {
                            Destroy(transform.GetChild(0).gameObject);
                        }
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
                    if (transform.childCount > 0) {
                        Destroy(transform.GetChild(0).gameObject);
                    }
                    break;
                }
            }
        }
    }

    public void AddNextToDoListItemFromUncraftedList() {
        if (itemInfo && !IsItemAlreadyInToDoList() && ToDoManager.instance.toDoList.Count < 4) {
            ToDoManager.instance.SetNewToDoList(itemInfo);
            GameObject newBorder = Instantiate(toDoListActiveBorderPrefab, transform.position, transform.rotation);
            newBorder.transform.SetParent(this.transform);
            newBorder.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
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


    public void UpdateEncyclopediaToolTip() {

        if (!ToolTipManager.instance.isMaximized) { return; }

        if (isEncyclopediaIcon) {
            if (shownItemsContainer == null) {
                shownItemsContainer = GameObject.Find("ShownItemsContainer");
            }

            shownItemsContainer.SetActive(true);
            toolTipText = "Required Components:";
        
            if (itemInfo != null) {
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

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.instance.isOverUI = false;
        ToolTipManager.instance.ToggleToolTipOff();
        Encyclopedia.instance.OnPointerExitDelegate();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        ToolTipManager.instance.isOverUI = true;
        
        if (!ToolTipManager.instance.isMaximized) { return; }

        ToolTipManager.instance.ToggleToolTipOn();
        ToolTipManager.instance.UpdateValues(toolTipName, toolTipText, 0, 0, 0, 0, 0);

        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManager.instance.isOverUI = true;

        if (!ToolTipManager.instance.isMaximized) { return; }

        ToolTipManager.instance.ToggleToolTipOn();
        ToolTipManager.instance.UpdateValues(toolTipName, toolTipText, 0, 0, 0, 0, 0);

        if (isEncyclopediaIcon) {
            UpdateEncyclopediaToolTip();
        }
    }
}
