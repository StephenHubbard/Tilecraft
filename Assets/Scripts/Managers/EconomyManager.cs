using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private int startingCoins = 3;
    [SerializeField] public int currentCoins;
    [SerializeField] private GameObject spinningCoinPrefab;

    public static EconomyManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Start() {
        currentCoins = startingCoins;
    }

    private void Update() {
        coinText.text = currentCoins.ToString();
    }

    public void SellItem(GameObject thisObj, int amount, int stackSize) {
        if (thisObj.GetComponent<Stackable>().isSellable == false) { return; }
        currentCoins += amount * stackSize;
        AudioManager.instance.Play("Sell");
        GameObject thisCoin = Instantiate(spinningCoinPrefab, thisObj.transform.position + new Vector3(0, 1f, 0), transform.rotation);
        StartCoroutine(DestroyCoinCo(thisCoin));
        Destroy(thisObj);
    }

    private IEnumerator DestroyCoinCo(GameObject thisCoin) {
        yield return new WaitForSeconds(.7f);
        Destroy(thisCoin);
    }

    public void BuyPack(int amount) {
        currentCoins -= amount;
        AudioManager.instance.Play("Pop");
    }

    
}
