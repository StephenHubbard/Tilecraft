using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class Fridge : MonoBehaviour
{
    [SerializeField] private Transform amountOfFoodContainer;
    [SerializeField] private TMP_Text amountOfFoodText;
    [SerializeField] public int currentAmountOfFood;

    private void Start() {
        currentAmountOfFood = 0;
    }

    private void OnMouseEnter() {
        ShowAmountOfFood();
    }

    private void OnMouseExit() {
        HideAmountOfFood();
    }

    public void IncreaseFoodAmount(int amount, int amountInStack) {
        currentAmountOfFood += (amount * amountInStack);

        StartCoroutine(FindObjectOfType<FoodManager>().UpdateFoodCo());

    }

    public void DecreaseFoodAmount(int amount) {
        currentAmountOfFood -= amount;
    }

    public void ShowAmountOfFood() {
        amountOfFoodContainer.gameObject.SetActive(true);
        amountOfFoodText.text = currentAmountOfFood.ToString();
    }

    public void HideAmountOfFood() {
        amountOfFoodContainer.gameObject.SetActive(false);
    }
}
