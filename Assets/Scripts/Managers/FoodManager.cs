using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodManager : MonoBehaviour
{
    [SerializeField] private TMP_Text foodText;
    [SerializeField] public int currentFood;
    [SerializeField] public int totalFoodNeeded;

    private void Start() {
        StartCoroutine(ResetDayCo());
    }

    private void Update() {
        foodText.text = currentFood.ToString() + "/" + totalFoodNeeded;
    }

    public IEnumerator UpdateFoodNeededCo() {
        yield return new WaitForEndOfFrame();
        UpdateAmountOfTotalFoodNeed();
    }

    public void UpdateAmountOfTotalFoodNeed() {
        Worker[] allWorkers = FindObjectsOfType<Worker>();

        totalFoodNeeded = allWorkers.Length * 2;
    }

    public IEnumerator UpdateFoodCo() {
        yield return new WaitForEndOfFrame();
        UpdateAmountOfCurrentFoodAvailable();
    }

    public void UpdateAmountOfCurrentFoodAvailable() {
        Food[] allFood = FindObjectsOfType<Food>();

        int amountOfFoodAvailable = 0;

        foreach (Food item in allFood)
        {
            amountOfFoodAvailable += item.foodWorthAmount;
        }

        Fridge[] allFridges = FindObjectsOfType<Fridge>();

        foreach (var fridge in allFridges)
        {
            amountOfFoodAvailable += fridge.currentAmountOfFood;
        }

        currentFood = amountOfFoodAvailable;
    }

    public void NewDayEatFood() {
        Food[] allFood = FindObjectsOfType<Food>();

        Fridge[] allFridges = FindObjectsOfType<Fridge>();

        int amountOfFoodNeededNow = totalFoodNeeded;
        bool enoughFood = false;

        foreach (var fridge in allFridges)
        {
            if (amountOfFoodNeededNow > 0) {
                for (int i = fridge.currentAmountOfFood; i > 0; i--)
                {
                    if (amountOfFoodNeededNow > 0) {
                        amountOfFoodNeededNow--;
                        fridge.currentAmountOfFood--;
                    }
                }
            }
        }

        foreach (Food item in allFood)
        {
            if (amountOfFoodNeededNow > 0) {
                amountOfFoodNeededNow -= item.foodWorthAmount;
                Destroy(item.gameObject);
            } else {
                enoughFood = true;
                break;
            }
        }

        if (!enoughFood) {
            CalcStarvedWorkers(currentFood - totalFoodNeeded);
        }

        StartCoroutine(ResetDayCo());
    }

    private IEnumerator ResetDayCo() {
        yield return new WaitForEndOfFrame();

        UpdateAmountOfCurrentFoodAvailable();
        UpdateAmountOfTotalFoodNeed();

        StartCoroutine(UpdateFoodCo());
    }

    private void CalcStarvedWorkers(int foodDeficitAmount) {
        print("lacking " + foodDeficitAmount + " food");

        Worker[] allWorkers = FindObjectsOfType<Worker>();

        int amountOfWorkersStarved = 0;
        int workersToStarve = Mathf.CeilToInt((float)-foodDeficitAmount / 2);

        for (int i = 0; i < workersToStarve; i++)
        {
            amountOfWorkersStarved++;
            if (allWorkers[i].transform.root.GetComponent<CraftingManager>()) {
                allWorkers[i].transform.root.GetComponent<CraftingManager>().DecreaseWorkerCount();
            }
            Destroy(allWorkers[i].gameObject);
        }

        print(amountOfWorkersStarved + " have starved");
        StartCoroutine(CheckGameOver());
    }

    private IEnumerator CheckGameOver() {
        yield return new WaitForEndOfFrame();

        Worker[] allWorkers = FindObjectsOfType<Worker>();

        if (allWorkers.Length == 0) {
            print("game over");
        }
    }
}
