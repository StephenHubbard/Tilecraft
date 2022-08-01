using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlacedItem : MonoBehaviour
{
    [SerializeField] public ItemInfo itemInfo;

    public bool CheckForValidRecipe() {
        CraftingManager craftingManager = GetComponentInParent<CraftingManager>();

        foreach (var item in GetComponentInParent<Tile>().currentPlacedResources)
        {
            print(item.name);   
        }

        foreach (var potentialOffSpring in itemInfo.potentialOffSpring)
        {
            List<ItemInfo> resourcesNeededForRecipeLookup = new List<ItemInfo>();
            
            foreach (var neededRecipeItemsInOffspring in potentialOffSpring.recipeInfo.neededRecipeItems)
            {
                resourcesNeededForRecipeLookup.Add(neededRecipeItemsInOffspring);
            }

            

            if (AreListsEqual(GetComponentInParent<Tile>().currentPlacedResources, resourcesNeededForRecipeLookup)) {
                GetComponentInParent<CraftingManager>().UpdateAmountLeftToCraft(potentialOffSpring.recipeInfo.itemInfo);
                GetComponentInParent<CraftingManager>().recipeInfo = potentialOffSpring.recipeInfo;
                GetComponentInParent<CraftingManager>().hasCompleteRecipe = true;
                return true;
            }

        }

        return false;
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
