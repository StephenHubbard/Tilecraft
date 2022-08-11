using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public int foodWorthAmount;

    private void Start() {
        if (GetComponent<DraggableItem>()) {
            foodWorthAmount = GetComponent<DraggableItem>().itemInfo.foodValue;
        } else if (GetComponent<PlacedItem>()) {
            foodWorthAmount = GetComponent<PlacedItem>().itemInfo.foodValue;
        }
    }
}
