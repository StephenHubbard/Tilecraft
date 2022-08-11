using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierOneCollider : MonoBehaviour
{
    private void Start() {
        StartCoroutine(DestroyObject());
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!other.gameObject.GetComponent<Tile>()) { return; }

        if (other.gameObject.GetComponent<Tile>().currentPlacedItem && other.gameObject.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>() && other.gameObject.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.itemName == "Orc Relic") {
            other.gameObject.GetComponent<Tile>().isOccupiedWithBuilding = false;
            other.gameObject.GetComponent<Tile>().currentPlacedResources.Clear();
            other.gameObject.GetComponent<Tile>().itemInfo = null;
            other.gameObject.GetComponent<CraftingManager>().hasCompleteRecipe = false;
            other.gameObject.GetComponent<CraftingManager>().recipeInfo = null;
            Destroy(other.gameObject.GetComponent<Tile>().currentPlacedItem);
        }

        if (other.gameObject.GetComponent<Tile>().currentPlacedItem && other.gameObject.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>() && other.gameObject.GetComponent<Tile>().currentPlacedItem.GetComponent<PlacedItem>().itemInfo.itemName == "Glacier") {
            other.gameObject.GetComponent<Tile>().isOccupiedWithBuilding = false;
            other.gameObject.GetComponent<Tile>().currentPlacedResources.Clear();
            other.gameObject.GetComponent<Tile>().itemInfo = null;
            other.gameObject.GetComponent<CraftingManager>().hasCompleteRecipe = false;
            other.gameObject.GetComponent<CraftingManager>().recipeInfo = null;
            Destroy(other.gameObject.GetComponent<Tile>().currentPlacedItem);
        }
    }

    private IEnumerator DestroyObject() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
