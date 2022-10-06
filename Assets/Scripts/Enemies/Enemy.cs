using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string id;

    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] public int myHealth = 3;
    [SerializeField] private LayerMask cloudLayerMask = new LayerMask();
    public Animator myAnimator;
    public Transform currentTarget = null;
    public bool isUncoveredByClouds = false;

    public bool isSkeletonWarrior = false;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }
    
    private void Start() {
        GenerateGuid();
        AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
        myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data) {

    }

    public void SaveData(ref GameData data) {
        if (data.isSkeletonWarrior.ContainsKey(id)) {
            data.isSkeletonWarrior.Remove(id);
            data.enemyPos.Remove(id);
        }
        data.isSkeletonWarrior.Add(id, isSkeletonWarrior);
        data.enemyPos.Add(id, transform.position);
    }

    private void Update() {

        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.zero, 100f, cloudLayerMask);

        if (hit.Length == 0) {
            isUncoveredByClouds = true;
        }
    }

    public void TakeDamage(int amount, Transform attackingWorker) {
        myHealth -= amount;

        if (myHealth <= 0) {
            if (attackingWorker.GetComponent<Worker>()) {
                attackingWorker.GetComponent<Worker>().CurrentEnemyNull();
            }

            if (attackingWorker.GetComponent<Knight>()) {
                attackingWorker.GetComponent<Knight>().CurrentEnemyNull();
            }

            if (attackingWorker.GetComponent<Archer>()) {
                attackingWorker.GetComponent<Archer>().CurrentEnemyNull();
            }

            GetComponentInParent<Tile>().currentPlacedItem.GetComponent<OrcRelic>().DetectIfEnemies();
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        if (isSkeletonWarrior) {
            Worker worker = other.gameObject.GetComponent<Worker>();
            if (worker && currentTarget == null && worker.GetComponent<PlacedItem>() && isUncoveredByClouds) {
                if (worker.GetComponentInParent<Tile>() == GetComponentInParent<Tile>()) {
                    myAnimator.SetBool("isAttacking", true);
                    myAnimator.Play("Attack", -1, Random.Range(0f,1f));
                    currentTarget = worker.transform;
                    AudioManager.instance.Play("Orc Attack");
                }
            }

            if (!isSkeletonWarrior) {
                Archer archer = other.gameObject.GetComponent<Archer>();
                if (archer && currentTarget == null && archer.GetComponent<PlacedItem>() && isUncoveredByClouds) {
                    myAnimator.SetBool("isAttacking", true);
                    myAnimator.Play("Attack", -1, Random.Range(0f,1f));
                    currentTarget = archer.transform;
                    AudioManager.instance.Play("Orc Attack");
                }
            }

            Knight knight = other.gameObject.GetComponent<Knight>();
            if (knight && currentTarget == null && knight.GetComponent<PlacedItem>() && isUncoveredByClouds) {
                if (knight.GetComponentInParent<Tile>() == GetComponentInParent<Tile>()) {
                    myAnimator.SetBool("isAttacking", true);
                    myAnimator.Play("Attack", -1, Random.Range(0f,1f));
                    currentTarget = knight.transform;
                    AudioManager.instance.Play("Orc Attack");
                }
            }
        } else {
            Worker worker = other.gameObject.GetComponent<Worker>();
            if (worker && currentTarget == null && worker.GetComponent<PlacedItem>() && isUncoveredByClouds) {
                myAnimator.SetBool("isAttacking", true);
                myAnimator.Play("Attack", -1, Random.Range(0f,1f));
                currentTarget = worker.transform;
                AudioManager.instance.Play("Orc Attack");
            }

            if (!isSkeletonWarrior) {
                Archer archer = other.gameObject.GetComponent<Archer>();
                if (archer && currentTarget == null && archer.GetComponent<PlacedItem>() && isUncoveredByClouds) {
                    myAnimator.SetBool("isAttacking", true);
                    myAnimator.Play("Attack", -1, Random.Range(0f,1f));
                    currentTarget = archer.transform;
                    AudioManager.instance.Play("Orc Attack");
                }
            }

            Knight knight = other.gameObject.GetComponent<Knight>();
            if (knight && currentTarget == null && knight.GetComponent<PlacedItem>() && isUncoveredByClouds) {
                myAnimator.SetBool("isAttacking", true);
                myAnimator.Play("Attack", -1, Random.Range(0f,1f));
                currentTarget = knight.transform;
                AudioManager.instance.Play("Orc Attack");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.GetComponent<Worker>() == currentTarget) {
            myAnimator.SetBool("isAttacking", false);
            currentTarget = null;
        }
    }

    public void ShootArrow() {
        if (currentTarget == null) { 
            myAnimator.SetBool("isAttacking", false);
            return; 
        }

        GameObject newArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        newArrow.GetComponent<Arrow>().UpdateCurrentTarget(currentTarget.transform);
        AudioManager.instance.Play("Arrow Launch");
    }

    public void dealClubDamage() {

        if (currentTarget == null) { 
            myAnimator.SetBool("isAttacking", false);
            return; 
        }

        if (currentTarget.GetComponent<Worker>()) {
            currentTarget.GetComponent<Worker>().TakeDamage(1, this);
            AudioManager.instance.Play("Orc Swing");
        }

        if (currentTarget.GetComponent<Knight>()) {
            currentTarget.GetComponent<Knight>().TakeDamage(1, this);
            AudioManager.instance.Play("Orc Swing");
        }

        if (currentTarget.GetComponent<Archer>()) {
            currentTarget.GetComponent<Archer>().TakeDamage(1, this);
            AudioManager.instance.Play("Orc Swing");
        }
    }

    
}
