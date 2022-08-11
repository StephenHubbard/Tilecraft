using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] private GameObject deadWorkerOnTilePrefab;
    [SerializeField] private GameObject deadWorkerItemPrefab;
    [SerializeField] private GameObject levelUpAnimPrefab;
    [SerializeField] private GameObject starGroup1;
    [SerializeField] private GameObject starGroup2;
    [SerializeField] private GameObject starGroup3;
    [SerializeField] public int myWorkingStrength = 1;
    [SerializeField] public int myCombatValue = 1;
    [SerializeField] public int foodNeededToUpPickaxeStrengthCurrent;
    [SerializeField] public int foodNeededToUpPickaxeStrengthStart = 3;
    public ItemInfo itemInfo;
    public int myHealth = 10;

    private Enemy enemyTarget = null;

    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start() {
        if (GetComponent<PlacedItem>() && myAnimator) {
            AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
            myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
        }

        DetectCombat();

        itemInfo.toolTipText = "strength value: " + myWorkingStrength.ToString();

        foodNeededToUpPickaxeStrengthCurrent = foodNeededToUpPickaxeStrengthStart;
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

    public void TransferStrength(int currentStrength) {
        myWorkingStrength = currentStrength;
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

    public void LevelUpStrength(int leftoverAmountOfFood) {
        GameObject starPrefab = Instantiate(levelUpAnimPrefab, transform.position, transform.rotation);
        StartCoroutine(DestroyStarPrefabCo(starPrefab));
        myWorkingStrength++;
        foodNeededToUpPickaxeStrengthStart *= Mathf.CeilToInt(1.5f);
        foodNeededToUpPickaxeStrengthCurrent = foodNeededToUpPickaxeStrengthStart;
        if (leftoverAmountOfFood > 0) {
            FeedWorker(leftoverAmountOfFood, false);
        }

    }

    private IEnumerator DestroyStarPrefabCo(GameObject starPrefab) {
        yield return new WaitForSeconds(2f);
        Destroy(starPrefab);
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
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount) {
        myHealth -= amount;
        DetectDeath();
    }

    public void StartWorking() {
        StartCoroutine(StartWorkingCo());
    }

    public void StopWorking() {
        StartCoroutine(StopWorkingCo());
    }

    public void TransferHealth(int currentHealth) {
        myHealth = currentHealth;
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
            enemyTarget.TakeDamage(1, this);
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
