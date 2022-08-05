using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] public int foodWorthAmount = 1;

    private void Start() {
        StartCoroutine(FindObjectOfType<FoodManager>().UpdateFoodCo());
    }
}
