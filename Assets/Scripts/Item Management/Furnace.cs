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
        craftingManager.UpdateAmountLeftToCraft(amountInStack);
        craftingManager.CheckCanStartCrafting();
    }

    public void AbandonSmelting() {
        for (int i = 0; i < amountLeftToSmelt; i++)
        {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(currentlySmeltingItem.draggableItemPrefab, spawnItemsVector3, transform.rotation);
        }
    }

    public void UpdateFurnaceAmountLeft() {
        amountLeftToSmelt--;
    }
}
