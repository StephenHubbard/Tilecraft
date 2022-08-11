using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HousingManager : MonoBehaviour
{
  [SerializeField] public int maximumWorkers;
  [SerializeField] public int currentWorkers;
  [SerializeField] private TMP_Text housingText;
  [SerializeField] private int startingWorkers;
  [SerializeField] private int housePopValue = 5;

  public static HousingManager instance;

  private void Awake() {
    if (instance == null) {
      instance = this;
    }
  }

  private void Start() {
    DetectHowManyWorkers();
    DetectHowManyHouses();
  }

  private void Update() {
    housingText.text = currentWorkers.ToString() + "/" + maximumWorkers.ToString();
  }

  public void DetectHowManyWorkers() {
    Worker[] amountOfWorkers = FindObjectsOfType<Worker>();
    currentWorkers = amountOfWorkers.Length;
  }

  public void DetectHowManyHouses() {
    House[] amountOfHouses = FindObjectsOfType<House>();
    maximumWorkers = (amountOfHouses.Length * housePopValue) + startingWorkers;
  }

  public void AddNewWorker() {
    currentWorkers++;
  }

  public void AddNewHouse(int amountHouseAdds) {
    maximumWorkers += amountHouseAdds;
  }
}
