using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Furnace : MonoBehaviour
{
    [SerializeField] private Transform currentResourceContainer;
    [SerializeField] private SpriteRenderer currentOccupiedSprite;
    [SerializeField] private TMP_Text amountText;

    public bool isAlter = false;

    public ItemInfo currentlySmeltingItem;
    public int amountLeftToSmelt;
    public bool occupiedWithResourceInFurnace = false;
    private CraftingManager craftingManager;
    public bool isSmelting = false;

    private void Update() {
        amountText.text = amountLeftToSmelt.ToString();
    }

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
        if (!currentlySmeltingItem) { return; }

        for (int i = 0; i < amountLeftToSmelt; i++)
        {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(currentlySmeltingItem.draggableItemPrefab, spawnItemsVector3, transform.rotation);
        }

        isSmelting = false;
        currentlySmeltingItem = null;
        occupiedWithResourceInFurnace = false;
        currentResourceContainer.gameObject.SetActive(false);
        GetComponentInParent<CraftingManager>().hasCompleteRecipe = false;
        GetComponentInParent<CraftingManager>().recipeInfo = null;

        AudioManager.instance.Play("Pop");
    }

    public void UpdateFurnaceAmountLeft() {
        amountLeftToSmelt--;
    }

    public void UpdateCurrentOccupiedResourceSprite(ItemInfo item) {
        currentResourceContainer.gameObject.SetActive(true);
        currentOccupiedSprite.sprite = item.itemSprite;
        currentlySmeltingItem = item;
    }

    public void HideOccupiedSpriteContainer() {
        currentResourceContainer.gameObject.SetActive(false);
    }
}
