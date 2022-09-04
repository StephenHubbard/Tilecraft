using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Create New Item")]
public class ItemInfo : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public int coinValue;
    public GameObject draggableItemPrefab;
    public GameObject onTilePrefab;
    public TileInfo[] tileInfoValidLocations;
    public RecipeInfo recipeInfo;
    public int amountRecipeCanCreate = 1;
    public ItemInfo[] potentialOffSpring;
    public bool isResourceOnly = true;
    public bool isStationary = false;
    public bool isSmeltable = false;
    public int foodValue = 0;
    public bool isPopulation = false;

    [TextArea]
    public string toolTipText;

    public TierGroup tierGroup;
    public enum TierGroup {
        one, 
        two, 
        three, 
        four, 
        five, 
        six
    };

    // OLD DISCOVERY SYSTEM
    public void NextInLineToDiscover() {
        // if (itemsNextToDiscover != null) {
        //     foreach (var item in itemsNextToDiscover)
        //     {
        //         Encyclopedia.instance.AddItemToDiscoveredList(item, true);
        //     }
        // }
    }

    public bool checkValidTiles(TileInfo tileInfo) { 
        foreach (var tile in tileInfoValidLocations)
        {
            if (tile.name == tileInfo.name) {
                return true;
            }
        }

        return false;
    }


}
