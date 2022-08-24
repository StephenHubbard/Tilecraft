using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PackManager : MonoBehaviour
{
    [SerializeField] public ItemInfo[] resourcePackItems;
    [SerializeField] private int amountOfItemsToSpawn = 4;
    [SerializeField] private GameObject nextPackToReveal;

    private int amountOfPacksOpened = 0;

    private EconomyManager economyManager;
    private Encyclopedia encyclopedia;

    private void Awake() {
        economyManager = FindObjectOfType<EconomyManager>();
        encyclopedia = FindObjectOfType<Encyclopedia>();
    }

    public void SpawnResourcePack() {
        if (economyManager.currentCoins < 3) {
            print("not enough coins");
        } else {
            for (int i = 0; i < amountOfItemsToSpawn; i++)
            {
                // z vector set to -1 for colliders and raycasts on interactable layer mask
                Vector3 packSpawnLocation = UtilsClass.GetMouseWorldPosition() + new Vector3(Random.Range(-3f, -5f), Random.Range(-3f, -5f), -1);
                if (resourcePackItems.Length > 0) {
                    int randomNum = Random.Range(0, resourcePackItems.Length);
                    Instantiate(resourcePackItems[randomNum].draggableItemPrefab, packSpawnLocation, transform.rotation);
                    encyclopedia.AddItemToDiscoveredList(resourcePackItems[randomNum]);
                }
            }
            amountOfPacksOpened++;
            NextPackAvailableCheck();
        }
    }

    private void NextPackAvailableCheck() {
        if (amountOfPacksOpened >= 3) {
            nextPackToReveal.SetActive(true);
        }
    }
}
