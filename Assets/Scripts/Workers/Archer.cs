using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string id;

    [SerializeField] private LayerMask cloudLayerMask = new LayerMask();
    [SerializeField] private LayerMask placedItemLayerMask = new LayerMask();

    [SerializeField] private GameObject deadWorkerItemPrefab;
    [SerializeField] private GameObject levelUpAnimPrefab;

    [SerializeField] public int myWorkingStrength = 1;
    [SerializeField] public int myCombatValue = 1;
    [SerializeField] public int foodNeededToUpCombatValue;
    [SerializeField] public int foodNeeded = 3;

    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject archerArrowPrefab;

    public Transform currentTarget = null;


    private Enemy enemyTarget = null;

    public int myHealth = 5;
    public int maxHealth = 4;


    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
        foodNeededToUpCombatValue = foodNeeded;
    }


    private void Start() {
        GenerateGuid();

        if (GetComponent<PlacedItem>() && myAnimator) {
            AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
            myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
        }

        OverlapCircleDetection();
        DetectCombat();
    }

    public void LoadData(GameData data) {

    }

    public void SaveData(ref GameData data) {
        if (GetComponent<DraggableItem>()) {
            if (data.draggablePopulationPos.ContainsKey(id)) {
                data.draggablePopulationPos.Remove(id);
            }
            data.draggablePopulationPos.Add(id, transform.position);
            data.draggableItemPopulation.Add(id, GetComponent<DraggableItem>().itemInfo);
        }

        if (GetComponent<PlacedItem>()) {
            if (data.placedItemsPopulationPos.ContainsKey(id)) {
                data.placedItemsPopulationPos.Remove(id);
            }
            data.placedItemsPopulationPos.Add(id, transform.position);
            data.placedItemPopulation.Add(id, GetComponent<PlacedItem>().itemInfo);
        }

        data.populationLevels.Add(id, GetComponent<Population>().currentLevel);
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    private void Update() {
        if (currentTarget == null && GetComponent<PlacedItem>()) {
            StopAttacking();
        }

        DetectCloudWhileWorking();
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
        myCombatValue = currentStrength;
        foodNeededToUpCombatValue = currentFoodNeeded;
        GetComponent<Population>().TransferLevel(currentLevel);
    }

        public void TransferHealth(int currentHealth, int currentMaxHealth) {
        myHealth = currentHealth;
        maxHealth = currentMaxHealth;
    }

    public void FeedArcher(int amount, bool playCrunch) {
        if (playCrunch) {
            AudioManager.instance.Play("Eat Crunch");
        }

        int leftoverAmountOfFood = 0;

        if (foodNeededToUpCombatValue < amount) {
            leftoverAmountOfFood = Mathf.Abs(foodNeededToUpCombatValue - amount);;
        }

        foodNeededToUpCombatValue -= amount;

        if (foodNeededToUpCombatValue <= 0) {
            LevelUpStrength(leftoverAmountOfFood, true);
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
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            GameObject newWorker = Instantiate(weapon.gameObject, spawnItemsVector3, transform.rotation);
            AudioManager.instance.Play("Pop");

        }

        if (weapon.weaponType == Weapon.WeaponType.pitchfork) {
            GameObject newWorker = Instantiate(workerPrefab, transform.position, transform.rotation);

            if (transform.childCount > 1) {
                transform.GetChild(1).transform.SetParent(null);
            }
            AudioManager.instance.Play("Pitchfork Attack");

            Destroy(gameObject);
        }

    }


    public void LevelUpStrength(int leftoverAmountOfFood, bool showLevelUpAnim) {
        if (showLevelUpAnim) {
            GameObject levelUpPrefabAnim = Instantiate(levelUpAnimPrefab, transform.position + new Vector3(0, 0, 0), transform.rotation);
            StartCoroutine(DestroyStarPrefabCo(levelUpPrefabAnim));
        }
        myCombatValue++;
        maxHealth++;
        myHealth = maxHealth;
        foodNeededToUpCombatValue = foodNeeded;
        if (leftoverAmountOfFood > 0) {
            FeedArcher(leftoverAmountOfFood, false);
        }
        GetComponent<Population>().UpLevelStars(true);
        DetermineFoodNeeded();
    }

    private void DetermineFoodNeeded() {
        
        if (GetComponent<Population>().currentLevel == 1) {
            foodNeededToUpCombatValue = 4;
        }

        if (GetComponent<Population>().currentLevel == 2) {
            foodNeededToUpCombatValue = 5;
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

    public void StartAttacking() {
        // myAnimator.Play("Attack", -1, Random.Range(0f,1f));
        myAnimator.SetBool("isAttacking", true);
    }

    public void StopAttacking() {
        myAnimator.SetBool("isAttacking", false);
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

    public void FireBow() {
        if (currentTarget == null) { return; }

        GameObject newArrow = Instantiate(archerArrowPrefab, transform.position, transform.rotation);
        newArrow.GetComponent<ArcherArrow>().archerThatLaunchedArrow = this.transform;
        newArrow.GetComponent<ArcherArrow>().UpdateCurrentTarget(currentTarget.transform);
        AudioManager.instance.Play("Arrow Launch");
    }

    public void OverlapCircleDetection() {
        Collider2D[] foundEnemies = Physics2D.OverlapCircleAll(transform.position, 1.2f, placedItemLayerMask);

        Enemy enemy = null;

        foreach (var item in foundEnemies)
        {
            if (item.GetComponent<Enemy>()) {
                enemy = item.GetComponent<Enemy>();
                break;
            }
        }

        if (enemy && currentTarget == null && enemy.GetComponent<PlacedItem>() && enemy.isUncoveredByClouds) {
            if (myAnimator) {
                myAnimator.SetBool("isAttacking", true);
                currentTarget = enemy.transform;
            }
        }
    }
}
