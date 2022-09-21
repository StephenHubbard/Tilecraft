using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlacedItem : MonoBehaviour, IDataPersistence
{   
    [SerializeField] public string id;

    [SerializeField] public ItemInfo itemInfo;
    public int amountLeft;

    private void Awake() {
        amountLeft = itemInfo.amountRecipeCanCreate;
    }

    private void Start() {
        CompleteFarmTutorial();
        GenerateGuid();
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data) {

    }

    public void SaveData(ref GameData data) {
        if (itemInfo.isResourceOnly) {
            if (data.tilePositions.ContainsKey(id)) {
                data.tilePositions.Remove(id);
            }
            data.placedItems.Add(id, itemInfo);
            data.placedItemsPos.Add(id, transform.position);
        }
    }

    private void CompleteFarmTutorial() {
        if (itemInfo.itemName == "Farm" && TutorialManager.instance.tutorialIndexNum == 2) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    public bool CheckForValidRecipe() {
        CraftingManager craftingManager = GetComponentInParent<CraftingManager>();


        foreach (var potentialOffSpring in itemInfo.potentialOffSpring)
        {
            List<ItemInfo> resourcesNeededForRecipeLookup = new List<ItemInfo>();
            
            foreach (var neededRecipeItemsInOffspring in potentialOffSpring.recipeInfo.neededRecipeItems)
            {
                resourcesNeededForRecipeLookup.Add(neededRecipeItemsInOffspring);
            }

            if (AreListsEqual(GetComponentInParent<Tile>().currentPlacedResources, resourcesNeededForRecipeLookup)) {
                GetComponentInParent<CraftingManager>().UpdateAmountLeftToCraft(amountLeft);
                GetComponentInParent<CraftingManager>().recipeInfo = potentialOffSpring.recipeInfo;
                GetComponentInParent<CraftingManager>().hasCompleteRecipe = true;
                return true;
            }

            if (GetComponent<Food>()) {
                GetComponentInParent<CraftingManager>().UpdateAmountLeftToCraft(1);
            }

        }

        GetComponentInParent<CraftingManager>().hasCompleteRecipe = false;
        return false;
    }

    public void UpdateAmountLeftToHarvest(int amountLeft) {
        this.amountLeft = amountLeft;
    }

    public static bool AreListsEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2) {
        var cnt = new Dictionary<T, int>();
        foreach (T s in list1) {
            if (cnt.ContainsKey(s)) {
            cnt[s]++;
            } else {
            cnt.Add(s, 1);
            }
        }
        foreach (T s in list2) {
            if (cnt.ContainsKey(s)) {
            cnt[s]--;
            } else {
            return false;
            }
        }
        return cnt.Values.All(c => c == 0);
    }

}
