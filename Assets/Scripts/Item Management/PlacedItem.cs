using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlacedItem : MonoBehaviour
{
    [SerializeField] public ItemInfo itemInfo;
    public int amountLeft;

    private void Awake() {
        amountLeft = 1;
    }

    private void Start() {
        CompleteTowerTutorial();
        ShowAutoSellToolTip();
    }

    private void CompleteTowerTutorial() {
        if (itemInfo.itemName == "Tower" && TutorialManager.instance.tutorialIndexNum == 2) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    private void ShowAutoSellToolTip() {
        if (itemInfo.amountRecipeCanCreate > 10 && itemInfo.itemName != "Alter") {
            TutorialManager.instance.ShowAutoSellTutorial();
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
