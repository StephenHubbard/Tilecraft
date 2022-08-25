using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class HousingManager : MonoBehaviour
{
  [SerializeField] public int maximumPopulation;
  [SerializeField] public int currentPopulation;
  [SerializeField] private int startingPopulation;
  [SerializeField] private int housePopValue = 5;
  [SerializeField] private Slider slider;
  [SerializeField] private TMP_Text housingText;
  [SerializeField] private Animator housingUIAnimator;

  public static HousingManager instance;

  private void Awake() {
    if (instance == null) {
      instance = this;
    }
  }

  private void Start() {
    DetectHowManyHouses();
    DetectTotalPopulation();
  }

  private void Update() {
    housingText.text = currentPopulation.ToString() + "/" + maximumPopulation.ToString();
    slider.maxValue = maximumPopulation;
    slider.value = currentPopulation;
  }

  public void DetectTotalPopulation() {
    StartCoroutine(GetAmountOfTotalPopulationEndOfFrameCo());
  }


  public void DetectHowManyHouses() {
    House[] amountOfHouses = FindObjectsOfType<House>();
    maximumPopulation = (amountOfHouses.Length * housePopValue) + startingPopulation;
  }

  public void BlinkNotEnoughWorkersAnim() {
    housingUIAnimator.SetTrigger("notEnoughHousing");
  }

  public void AddNewWorker() {
    currentPopulation++;
    StartCoroutine(GetAmountOfTotalPopulationEndOfFrameCo());
  }

  public IEnumerator GetAmountOfTotalPopulationEndOfFrameCo() {
    yield return new WaitForEndOfFrame();
    Population[] totalPop = FindObjectsOfType<Population>();
    currentPopulation = totalPop.Length;
  }

  public void AddNewHouse(int amountHouseAdds) {
    maximumPopulation += amountHouseAdds;
  }

  public void AllHousesDetectBabyMaking() {
    StartCoroutine(BabyMakingEndOfFrameCo());
  }

  private IEnumerator BabyMakingEndOfFrameCo() {
    yield return new WaitForEndOfFrame();

    House[] allHouses = FindObjectsOfType<House>();
    foreach (House house in allHouses)
    {
      house.DetectBabyMaking();
    }
  }
}
