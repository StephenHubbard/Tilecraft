using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
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
        if (GetComponent<PlacedItem>() && myAnimator) {
            AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
            myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
        }

        DetectCombat();
    }

    private void Update() {
        if (currentTarget == null && GetComponent<PlacedItem>()) {
            StopAttacking();
        }
    }

    private void DetectCombat() {
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

    public void TransferStrength(int currentStrength, int currentFoodNeeded) {
        myCombatValue = currentStrength;
        foodNeededToUpCombatValue = currentFoodNeeded;
    }

        public void TransferHealth(int currentHealth) {
        myHealth = currentHealth;
    }

    public void FeedWorker(int amount, bool playCrunch) {
        if (playCrunch) {
            AudioManager.instance.Play("Eat Crunch");
        }

        int leftoverAmountOfFood = 0;

        if (foodNeededToUpCombatValue < amount) {
            leftoverAmountOfFood = Mathf.Abs(foodNeededToUpCombatValue - amount);;
        }

        foodNeededToUpCombatValue -= amount;

        if (foodNeededToUpCombatValue <= 0) {
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


    public void LevelUpStrength(int leftoverAmountOfFood) {
        EconomyManager.instance.CheckDiscovery(1);
        GameObject levelUpPrefabAnim = Instantiate(levelUpAnimPrefab, transform.position + new Vector3(0, .5f, 0), transform.rotation);
        StartCoroutine(DestroyStarPrefabCo(levelUpPrefabAnim));
        myCombatValue++;
        maxHealth++;
        myHealth = maxHealth;
        foodNeeded *= Mathf.CeilToInt(1.5f);
        foodNeededToUpCombatValue = foodNeeded;
        if (leftoverAmountOfFood > 0) {
            FeedWorker(leftoverAmountOfFood, false);
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

    private void DetectDeath() {
        if (myHealth <= 0) {
            GetComponentInParent<CraftingManager>().AllWorkersHaveDiedCheck();
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            Instantiate(deadWorkerItemPrefab, spawnItemsVector3, transform.rotation);    
            AudioManager.instance.Play("Worker Death");
            HousingManager.instance.DetectTotalPopulation();
            HousingManager.instance.AllHousesDetectBabyMaking();
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount) {
        myHealth -= amount;
        DetectDeath();
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

    private void OnCollisionStay2D(Collision2D other) {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy && currentTarget == null && enemy.GetComponent<PlacedItem>() && enemy.isUncoveredByClouds) {
            if (myAnimator) {
                myAnimator.SetBool("isAttacking", true);
                currentTarget = enemy.transform;
            }
        }
    }
}
