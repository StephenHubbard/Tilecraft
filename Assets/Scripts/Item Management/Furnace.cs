using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    private ItemInfo currentlySmeltingItem;
    private int amountLeftToSmelt;

    private CraftingManager craftingManager;


    public void StartSmelting(ItemInfo itemInfo, int amountInStack) {
        craftingManager = GetComponentInParent<CraftingManager>();
        amountLeftToSmelt = amountInStack;
        currentlySmeltingItem = itemInfo;
        craftingManager.recipeInfo = currentlySmeltingItem.potentialOffSpring[0].recipeInfo;
        craftingManager.hasCompleteRecipe = true;
        craftingManager.UpdateAmountLeftToCraft(amountLeftToSmelt);
        craftingManager.CheckCanStartCrafting();
    }
}
