using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Create New Recipe")]
public class RecipeInfo : ScriptableObject
{
    public ItemInfo itemInfo;
    public ItemInfo[] neededRecipeItems;
    public float recipeCraftTime;
    public string craftingClipString;

    
}
