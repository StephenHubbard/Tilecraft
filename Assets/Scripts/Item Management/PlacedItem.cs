using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedItem : MonoBehaviour
{
    [SerializeField] public ItemInfo itemInfo;
    // put in what is crafted from the placedItem
    [SerializeField] public RecipeInfo offspringRecipeInfo;

    public bool CheckForValidRecipe() {
        if (offspringRecipeInfo) {
            foreach (var recipe in offspringRecipeInfo.neededRecipeItems)
            {
                if (recipe == itemInfo) { 
                    GetComponentInParent<CraftingManager>().hasCompleteRecipe = true;
                    GetComponentInParent<CraftingManager>().recipeInfo = offspringRecipeInfo;
                    return true;
                } 
            }

            GetComponentInParent<CraftingManager>().recipeInfo = offspringRecipeInfo;
            return false;
        }

        return false;
    }

}
