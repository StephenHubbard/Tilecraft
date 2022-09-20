using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Worker : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string id;
    [SerializeField] private LayerMask cloudLayerMask = new LayerMask();
    [SerializeField] private GameObject deadWorkerOnTilePrefab;
    [SerializeField] private GameObject deadWorkerItemPrefab;
    [SerializeField] private GameObject levelUpAnimPrefab;
    [SerializeField] public int myWorkingStrength = 1;
    [SerializeField] public int myCombatValue = 1;
    [SerializeField] private GameObject archerPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] public int foodNeededToUpPickaxeStrengthCurrent;
    [SerializeField] public int foodNeeded = 3;
    public ItemInfo itemInfo;
    public int myHealth;
    public int maxHealth = 3;

    public bool isBabyMaking = false;

    private Enemy enemyTarget = null;

    public Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
        foodNeededToUpPickaxeStrengthCurrent = foodNeeded;
    }

    private void Start() {
        GenerateGuid();

        if (GetComponent<PlacedItem>() && myAnimator) {
            AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
            myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
        }

        DetectCombat();

        itemInfo.toolTipText = "strength value: " + myWorkingStrength.ToString();
    }

    public void LoadData(GameData data) {

    }

    public void SaveData(ref GameData data) {
        if (GetComponent<DraggableItem>()) {
            if (data.draggableItemWorkersPos.ContainsKey(id)) {
                data.draggableItemWorkersPos.Remove(id);
            }
            data.draggableItemWorkersPos.Add(id, transform.position);
            data.draggableItemWorkers.Add(id, itemInfo);
        }

        if (GetComponent<PlacedItem>()) {
            if (data.placedItemsWorkersPos.ContainsKey(id)) {
                data.placedItemsWorkersPos.Remove(id);
            }
            data.placedItemsWorkersPos.Add(id, transform.position);
            data.placedItemWorkers.Add(id, itemInfo);
        }
    }

    private void Update()
    {
        DetectCloudWhileWorking();
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    private void DetectCloudWhileWorking()
    {
        if (!GetComponent<PlacedItem>()) { return; }

        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, Vector2.zero, 100f, cloudLayerMask);
        RaycastHit2D[] hitArrayTwo = Physics2D.RaycastAll(transform.position + new Vector3(0, 1, 0), Vector2.zero, 100f, cloudLayerMask);

        if (hitArray.Length > 0)
        {
            foreach (var cloud in hitArray)
            {

                cloud.transform.gameObject.GetComponent<Animator>().SetBool("HalfFade", true);
            }
        }

        if (hitArrayTwo.Length > 0)
        {
            foreach (var cloud in hitArrayTwo)
            {

                cloud.transform.gameObject.GetComponent<Animator>().SetBool("HalfFade", true);
            }
        }
    }

    private void OnDestroy() {
        if (!GetComponent<PlacedItem>()) { return; }

        RaycastHit2D[] hitArray = Physics2D.RaycastAll(transform.position, Vector2.zero, 100f, cloudLayerMask);
        RaycastHit2D[] hitArrayTwo = Physics2D.RaycastAll(transform.position + new Vector3(0, 1, 0), Vector2.zero, 100f, cloudLayerMask);

        if (hitArray.Length > 0)
        {
            foreach (var cloud in hitArray)
            {

                cloud.transform.gameObject.GetComponent<Animator>().SetBool("HalfFade", false);
            }
        }

        if (hitArrayTwo.Length > 0)
        {
            foreach (var cloud in hitArrayTwo)
            {

                cloud.transform.gameObject.GetComponent<Animator>().SetBool("HalfFade", false);
            }
        }
    }

    public void DetectCombat() {
            if (transform.GetComponent<PlacedItem>() && transform.root.GetComponent<Tile>().currentPlacedItem && transform.root.GetComponent<Tile>().currentPlacedItem.GetComponent<OrcRelic>() && enemyTarget == null) {

                bool isOccupiedWithEnemies = false;

                foreach (var orcSpawnPoint in transform.root.GetComponent<Tile>().currentPlacedItem.GetComponent<OrcRelic>().orcSpawnPoints)
                {
                    if (orcSpawnPoint.childCount > 0) {
                        isOccupiedWithEnemies = true;
                    }
                }
                
                if (isOccupiedWithEnemies) {
                    StartAttacking();

                    FindEnemy();
                }
        }
    }

    public void TransferStrength(int currentStrength, int currentFoodNeeded, int currentLevel) {
        myWorkingStrength = currentStrength;
        foodNeededToUpPickaxeStrengthCurrent = currentFoodNeeded;
        GetComponent<Population>().TransferLevel(currentLevel);
    }
    

    public void FeedWorker(int amount, bool playCrunch) {
        if (playCrunch) {
            AudioManager.instance.Play("Eat Crunch");
        }

        int leftoverAmountOfFood = 0;

        if (foodNeededToUpPickaxeStrengthCurrent < amount) {
            leftoverAmountOfFood = Mathf.Abs(foodNeededToUpPickaxeStrengthCurrent - amount);;
        }

        foodNeededToUpPickaxeStrengthCurrent -= amount;

        if (foodNeededToUpPickaxeStrengthCurrent <= 0) {
            LevelUpStrength(leftoverAmountOfFood);
        }
    }

    public void EquipWorker(Weapon weapon) {

        if (weapon.weaponType == Weapon.WeaponType.sword) {
            GameObject newWorker = Instantiate(knightPrefab, transform.position, transform.rotation);

            if (transform.childCount > 1) {
                transform.GetChild(1).transform.SetParent(null);
            }
            AudioManager.instance.Play("Knight Equip");
            Destroy(gameObject);
        }

        if (weapon.weaponType == Weapon.WeaponType.bow) {
            GameObject newWorker = Instantiate(archerPrefab, transform.position, transform.rotation);

            if (transform.childCount > 1) {
                transform.GetChild(1).transform.SetParent(null);
            }
            AudioManager.instance.Play("Archer Equip");
            Destroy(gameObject);
        }

        if (weapon.weaponType == Weapon.WeaponType.pitchfork) {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            GameObject newWorker = Instantiate(weapon.gameObject, spawnItemsVector3, transform.rotation);
            AudioManager.instance.Play("Pop");
        }
    }


    public void LevelUpStrength(int leftoverAmountOfFood) {
        // EconomyManager.instance.CheckDiscovery(1);
        
        GameObject levelUpPrefabAnim = Instantiate(levelUpAnimPrefab, transform.position + new Vector3(0, .5f, 0), transform.rotation);
        StartCoroutine(DestroyStarPrefabCo(levelUpPrefabAnim));
        myWorkingStrength++;
        maxHealth++;
        myHealth = maxHealth;
        foodNeededToUpPickaxeStrengthCurrent = foodNeeded;
        if (leftoverAmountOfFood > 0) {
            FeedWorker(leftoverAmountOfFood, false);
        }
        GetComponent<Population>().UpLevelStars(true);
        DetermineFoodNeeded();
    }

    private void DetermineFoodNeeded() {
        
        if (GetComponent<Population>().currentLevel == 1) {
            foodNeededToUpPickaxeStrengthCurrent = 4;
        }

        if (GetComponent<Population>().currentLevel == 2) {
            foodNeededToUpPickaxeStrengthCurrent = 5;
        }
    }
    

    private IEnumerator DestroyStarPrefabCo(GameObject levelUpPrefabAnim) {
        yield return new WaitForSeconds(2f);
        Destroy(levelUpPrefabAnim);
    }

    private void FindEnemy() {
        if (enemyTarget == null) {
                foreach (var orcSpawnPoint in transform.root.GetComponent<Tile>().currentPlacedItem.GetComponent<OrcRelic>().orcSpawnPoints)
                {
                    if (orcSpawnPoint.childCount > 0) {
                        enemyTarget = orcSpawnPoint.GetChild(0).GetComponent<Enemy>();
                        return;
                    }
                }
            }
    }

    private void DetectDeath(Enemy myEnemy) {
        if (myHealth <= 0) {
            GetComponentInParent<CraftingManager>().AllWorkersHaveDiedCheck();
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(deadWorkerItemPrefab, spawnItemsVector3, transform.rotation);    
            AudioManager.instance.Play("Worker Death");
            HousingManager.instance.DetectTotalPopulation();
            HousingManager.instance.AllHousesDetectBabyMaking();
            if (myEnemy != null) {
                myEnemy.myAnimator.SetBool("isAttacking", false);
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount, Enemy myEnemy) {
        myHealth -= amount;
        DetectDeath(myEnemy);
    }

    public void StartWorking() {
        StartCoroutine(StartWorkingCo());
    }

    public void StopWorking() {
        StartCoroutine(StopWorkingCo());
    }

    public void TransferHealth(int currentHealth, int currentMaxHealth) {
        myHealth = currentHealth;
        maxHealth = currentMaxHealth;
    }


    private IEnumerator StartWorkingCo() {
        yield return new WaitForSeconds(Random.Range(0f, .3f));
        myAnimator.SetBool("isWorking", true);
        GetComponentInParent<CraftingManager>().IncreaseWorkerCount();
    }

    private IEnumerator StopWorkingCo() {
        yield return new WaitForSeconds(Random.Range(0f, .4f));
        myAnimator.SetBool("isWorking", false);
    }

    public void StartAttacking() {
        // myAnimator.Play("Attack", -1, Random.Range(0f,1f));
        myAnimator.SetBool("isAttacking", true);
    }

    public void StopAttacking() {
        myAnimator.SetBool("isAttacking", false);
    }

    public void HitTarget() {
        if (enemyTarget) {
            enemyTarget.TakeDamage(myCombatValue, this.transform);
            AudioManager.instance.Play("Pitchfork Attack");

        } else {
            StopAttacking();
            StartCoroutine(DetectNewEnemyCo());
        }
    }

    public void CurrentEnemyNull() {
        enemyTarget = null;
        StopAttacking();
        StartCoroutine(DetectNewEnemyCo());
    }

    private IEnumerator DetectNewEnemyCo() {
        yield return new WaitForEndOfFrame();
        DetectCombat();
    }
}
