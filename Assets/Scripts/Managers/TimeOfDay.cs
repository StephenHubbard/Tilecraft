using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [SerializeField] private Transform handleParent;
    [SerializeField] private float timeLeftInDay;

    [Header("How long a day is in seconds")]
    [SerializeField] private float howLongIsOneDay;

    [SerializeField] private LayerMask cloudLayerMask = new LayerMask();
    [SerializeField] private GameObject newTileSmokePrefab;
    
    private float clockSpinFactor;
    private bool countDownStarted = false;

    private FoodManager foodManager;


    private void Awake() {
        foodManager = FindObjectOfType<FoodManager>();
    }

    private void Start() {
        clockSpinFactor = 360f / howLongIsOneDay ;
        timeLeftInDay = howLongIsOneDay;
    }

    private void Update() {
        timeLeftInDay -= Time.deltaTime;

        handleParent.transform.eulerAngles = new Vector3(0, 0, timeLeftInDay * clockSpinFactor);

        ResetDay();
        // ThreeSecondsLeft();
    }

    private void ThreeSecondsLeft() {
        if (timeLeftInDay <= 3f && !countDownStarted) {
            countDownStarted = true;
            StartCoroutine(TripleBeepDown());
        }
    }

    private IEnumerator TripleBeepDown() {
        int amountOfBeeps = 0;

        while (countDownStarted) {
            AudioManager.instance.Play("Beep");
            yield return new WaitForSeconds(1f);
            amountOfBeeps++;
            if (amountOfBeeps == 3) {
                countDownStarted = false;
                AudioManager.instance.Play("End Of Day Bell");
            }
        }

        yield return null;
    }


    private void ResetDay() {
        if (timeLeftInDay <= 0) {
            timeLeftInDay = howLongIsOneDay;

            AudioManager.instance.Play("End Of Day Bell");
            SpawnNewItems();
        }
    }

    private void SpawnNewItems() {
        List<ItemInfo> allSpawnableItems = new List<ItemInfo>();
        
        foreach (var item in WorldGeneration.instance.ReturnSpawnableItems())
        {
            if (item.itemName != "Orc Relic") {
                allSpawnableItems.Add(item);
            }
        }

        Tile[] allTiles = FindObjectsOfType<Tile>();

        List<Tile> freeTiles = new List<Tile>();

        foreach (var tile in allTiles)
        {
            RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, Vector2.zero, 100f, cloudLayerMask);

            if (!hit && tile.currentPlacedItem == null && tile.GetComponent<CraftingManager>().isCrafting == false) {
                freeTiles.Add(tile);
            }
        }


        foreach (var tile in freeTiles)
        {
            List<ItemInfo> potentialItems = new List<ItemInfo>();
            
            foreach (var potentialItem in allSpawnableItems)
            {
                foreach (var validLandTile in potentialItem.tileInfoValidLocations)
                {

                    if (validLandTile == tile.GetComponent<Tile>().tileInfo) {
                        potentialItems.Add(potentialItem);
                    }
                }
            }

            int whichPrefabToSpawnNum = Random.Range(0, potentialItems.Count);

            int spawnChance = Random.Range(0, 3);

            if (spawnChance == 1) {
                GameObject startingPlacedItem = Instantiate(potentialItems[whichPrefabToSpawnNum].onTilePrefab, tile.transform.position, transform.rotation);
                GameObject newTileSmoke = Instantiate(newTileSmokePrefab, tile.transform.position + new Vector3(0, .5f, 0), transform.rotation);
                StartCoroutine(DestroySmokePrefabCo(newTileSmoke));
                startingPlacedItem.transform.parent = tile.transform;
                tile.GetComponent<Tile>().UpdateCurrentPlacedItem(startingPlacedItem.GetComponent<PlacedItem>().itemInfo, startingPlacedItem);
                tile.GetComponent<Tile>().isOccupiedWithBuilding = true;
                tile.GetComponent<CraftingManager>().UpdateAmountLeftToCraft(startingPlacedItem.GetComponent<PlacedItem>().itemInfo.amountRecipeCanCreate);
                tile.GetComponent<CraftingManager>().CheckCanStartCrafting();
                FadeInNewTileItem(startingPlacedItem);
            }
        }
    }

    private IEnumerator DestroySmokePrefabCo(GameObject smokePrefab) {
        yield return new WaitForSeconds(2f);
        Destroy(smokePrefab);
    }

    private void FadeInNewTileItem(GameObject startingPlacedItem) {
        foreach (Transform child in startingPlacedItem.transform)
        {
            if (child.GetComponent<SpriteRenderer>()) {
                StartCoroutine(FadeInItemCo(child));
            }
        }
    }

    private IEnumerator FadeInItemCo(Transform itemToFadeIn) {
        Color itemAlpha = itemToFadeIn.GetComponent<SpriteRenderer>().color;

        itemAlpha.a = 0f;
        itemToFadeIn.GetComponent<SpriteRenderer>().color = itemAlpha;


        for (float alpha = 0f; alpha <= 1f; alpha += 0.05f)
        {
            itemAlpha.a = alpha;
            itemToFadeIn.GetComponent<SpriteRenderer>().color = itemAlpha;
            yield return null;
        }

    }


}
