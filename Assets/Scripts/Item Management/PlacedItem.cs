using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedItem : MonoBehaviour
{
    [SerializeField] public ItemInfo itemInfo;
    // put in what is crafted from the placedItem
    [SerializeField] public RecipeInfo recipeInfo;

    public bool CheckForValidRecipe() {
        foreach (var recipe in recipeInfo.neededRecipeItems)
        {
            if (recipe == itemInfo) { 
                GetComponentInParent<CraftingManager>().hasCompleteRecipe = true;
                GetComponentInParent<CraftingManager>().recipeInfo = recipeInfo;
                return true;
            } 
        }

        GetComponentInParent<CraftingManager>().recipeInfo = recipeInfo;
        return false;
    }

}
