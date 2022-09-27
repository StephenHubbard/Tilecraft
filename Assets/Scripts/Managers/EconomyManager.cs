using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EconomyManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] public int currentXP = 0;
    [SerializeField] public int xpTillDiscovery = 5;
    [SerializeField] private GameObject spinningCoinPrefab;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text coinText;

    [SerializeField] private GameObject lockOnXPContainer;

    public static EconomyManager instance;

    public bool allItemsDiscovered = false;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        slider.maxValue = xpTillDiscovery;
        slider.value = currentXP;
    }

    public void LoadData(GameData data) {
        this.currentXP = data.currentXP;
        this.xpTillDiscovery = data.xpTillDiscovery;
        slider.maxValue = xpTillDiscovery;
        slider.value = currentXP;
    }

    public void SaveData(ref GameData data) {
        data.currentXP = this.currentXP;
        data.xpTillDiscovery = this.xpTillDiscovery;
    }

    private void Update() {
        coinText.text = currentXP.ToString() + "/" + xpTillDiscovery.ToString();

    }

    public void CheckDiscovery(int amount) {
        if (allItemsDiscovered == true) { 
            slider.value = slider.maxValue;
            return; 
        }

        int leftoverAmount = (xpTillDiscovery - currentXP) - amount;

        currentXP += amount;
        slider.value = currentXP;

        if (currentXP >= xpTillDiscovery) {
            NewDiscovery();
            slider.maxValue = xpTillDiscovery + 1;
            xpTillDiscovery = (int)slider.maxValue;
            currentXP = 0;
            slider.value = currentXP;
        }

        if (leftoverAmount < 0) {
            CheckDiscovery(Mathf.Abs(leftoverAmount));
        }
    }

    public void AllItemsDiscovered() {
        allItemsDiscovered = true;
        lockOnXPContainer.SetActive(true);
        slider.maxValue = 1;
        slider.value = slider.maxValue;
        coinText.gameObject.SetActive(false);
        StartCoroutine(AllItemsDiscoveredEndOfFrameCo());
    }

    private IEnumerator AllItemsDiscoveredEndOfFrameCo() {
        yield return null;
        CheckDiscovery(0);
    }

    public void FarmXP(int amount) {
        CheckDiscovery(amount);
    }

    public void NewDiscovery() {
        DiscoveryManager.instance.DetermineNewDiscovery();
    }


    private IEnumerator DestroyCoinCo(GameObject thisCoin) {
        yield return new WaitForSeconds(.7f);
        Destroy(thisCoin);
    }

    
}
