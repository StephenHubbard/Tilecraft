using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] public int currentCoins = 0;
    [SerializeField] public int coinsTillDiscovery = 5;
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
        slider.maxValue = coinsTillDiscovery;
        slider.value = currentCoins;
    }

    private void Update() {
        coinText.text = currentCoins.ToString() + "/" + coinsTillDiscovery.ToString();

    }

    public void CheckDiscovery(int amount) {

        if (allItemsDiscovered == true) { 
            slider.value = slider.maxValue;
            return; 
        }

        int leftoverAmount = (coinsTillDiscovery - currentCoins) - amount;

        currentCoins += amount;
        slider.value = currentCoins;

        if (currentCoins >= coinsTillDiscovery) {
            NewDiscovery();
            slider.maxValue = coinsTillDiscovery + 1;
            coinsTillDiscovery = (int)slider.maxValue;
            currentCoins = 0;
            slider.value = currentCoins;
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
        yield return new WaitForEndOfFrame();
        CheckDiscovery(0);
    }

    public void FarmXP(int amount) {
        print("hit");
        CheckDiscovery(amount);
    }

    public void NewDiscovery() {
        DiscoveryManager.instance.DetermineNewDiscovery();
    }

    // public void SellItem(GameObject thisObj, int amount, int stackSize) {
    //     if (thisObj.GetComponent<Stackable>().isSellable == false) { return; }

    //     AudioManager.instance.Play("Sell");
    //     GameObject thisCoin = Instantiate(spinningCoinPrefab, thisObj.transform.position + new Vector3(0, 1f, 0), transform.rotation);
    //     StartCoroutine(DestroyCoinCo(thisCoin));
    //     CheckDiscovery(amount * stackSize);


    //     Destroy(thisObj);
    // }

    private IEnumerator DestroyCoinCo(GameObject thisCoin) {
        yield return new WaitForSeconds(.7f);
        Destroy(thisCoin);
    }

    
}
