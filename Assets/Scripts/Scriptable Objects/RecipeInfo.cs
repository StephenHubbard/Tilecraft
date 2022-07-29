using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Create New Recipe")]
public class RecipeInfo : ScriptableObject
{
    public new string name;
    public ItemInfo itemInfo;
    public ItemInfo[] neededRecipeItems;
    public float recipeCraftTime;
}
