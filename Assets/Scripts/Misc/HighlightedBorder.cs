using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightedBorder : MonoBehaviour
{
    [SerializeField] private Color greenColor;
    [SerializeField] private Color redColor;
    
    private SpriteRenderer spriteRenderer; 

    public ItemInfo currentHeldItem;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeToWhite() {
        spriteRenderer.color = Color.white;
    }

    public void ChangeToRed() {
        spriteRenderer.color = redColor;
    }

    public void ChangeToGreen() {
        spriteRenderer.color = greenColor;
    }

    public void UpdateCurrentItemInHand(ItemInfo itemInfo) {
        currentHeldItem = itemInfo;
    }

    public void checkIfHoveredOverTileIsValid(TileInfo tileInfo) { 
        if (currentHeldItem != null) {
            foreach (var tile in currentHeldItem.tileInfoValidLocations)
            {
                if (tile.name == tileInfo.name) {
                    print("valid tile");
                    ChangeToGreen();
                } else {
                    print("invalid tile");
                    ChangeToRed();
                }
            }
        }

    }
}
