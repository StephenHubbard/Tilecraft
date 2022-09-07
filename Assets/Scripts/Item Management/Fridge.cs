using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class Fridge : MonoBehaviour
{
    [SerializeField] private TMP_Text amountOfFoodText;
    [SerializeField] public int currentAmountOfFood;
    [SerializeField] private GameObject appleItemPrefab;

    private void Start() {
        currentAmountOfFood = 0;
    }

    private void Update() {
        amountOfFoodText.text = currentAmountOfFood.ToString();
    }


    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1) && currentAmountOfFood > 0) {
            AudioManager.instance.Play("Pop");
            Vector3 spawnItemsVector3 = transform.position + new Vector3(1f, 0, 0);
            Instantiate(appleItemPrefab, spawnItemsVector3, transform.rotation);
            currentAmountOfFood--;
        }
    }

    public void IncreaseFoodAmount(int amount, int amountInStack) {
        currentAmountOfFood += (amount * amountInStack);
        AudioManager.instance.Play("Click");

    }

    public void DecreaseFoodAmount(int amount) {
        currentAmountOfFood -= amount;
    }


}
