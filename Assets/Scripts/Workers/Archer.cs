using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : MonoBehaviour
{
    [SerializeField] private GameObject deadWorkerItemPrefab;
    [SerializeField] private GameObject levelUpAnimPrefab;

    [SerializeField] public int myWorkingStrength = 1;
    [SerializeField] public int myCombatValue = 1;
    [SerializeField] public int foodNeededToUpPickaxeStrengthCurrent;
    [SerializeField] public int foodNeededToUpPickaxeStrengthStart = 3;

    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private GameObject knightPrefab;

    private Enemy enemyTarget = null;


    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    public int myHealth = 5;

    private void Start() {
        if (GetComponent<PlacedItem>() && myAnimator) {
            AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
            myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
        }

        foodNeededToUpPickaxeStrengthCurrent = foodNeededToUpPickaxeStrengthStart;
        
    }

    public void TransferStrength(int currentStrength) {
        myWorkingStrength = currentStrength;
    }

        public void TransferHealth(int currentHealth) {
        myHealth = currentHealth;
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
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            GameObject newWorker = Instantiate(knightPrefab, spawnItemsVector3, transform.rotation);

            if (transform.childCount > 1) {
                transform.GetChild(1).transform.SetParent(null);
            }

            Destroy(gameObject);
        }

        if (weapon.weaponType == Weapon.WeaponType.bow) {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            GameObject newWorker = Instantiate(weapon.gameObject, spawnItemsVector3, transform.rotation);
        }

        if (weapon.weaponType == Weapon.WeaponType.pitchfork) {
            Vector3 spawnItemsVector3 = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), -1);
            GameObject newWorker = Instantiate(workerPrefab, spawnItemsVector3, transform.rotation);

            if (transform.childCount > 1) {
                transform.GetChild(1).transform.SetParent(null);
            }

            Destroy(gameObject);
        }

        AudioManager.instance.Play("Pop");
    }


    public void LevelUpStrength(int leftoverAmountOfFood) {
        GameObject levelUpPrefabAnim = Instantiate(levelUpAnimPrefab, transform.position, transform.rotation);
        StartCoroutine(DestroyStarPrefabCo(levelUpPrefabAnim));
        myWorkingStrength++;
        foodNeededToUpPickaxeStrengthStart *= Mathf.CeilToInt(1.5f);
        foodNeededToUpPickaxeStrengthCurrent = foodNeededToUpPickaxeStrengthStart;
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
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int amount) {
        myHealth -= amount;
        DetectDeath();
    }
}
