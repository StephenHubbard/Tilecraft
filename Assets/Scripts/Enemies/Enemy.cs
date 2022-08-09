using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int myHealth = 3;
    [SerializeField] private LayerMask cloudLayerMask = new LayerMask();
    private Animator myAnimator;
    private Worker currentTarget = null;
    public bool isUncoveredByClouds = false;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }
    
    private void Start() {
        AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
        myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
    }

    private void Update() {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.zero, 100f, cloudLayerMask);

        if (hit.Length == 0) {
            isUncoveredByClouds = true;
        }
    }

    public void TakeDamage(int amount, Worker attackingWorker) {
        myHealth -= amount;

        if (myHealth <= 0) {
            attackingWorker.CurrentEnemyNull();
            GetComponentInParent<Tile>().currentPlacedItem.GetComponent<OrcRelic>().DetectIfEnemies();
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D other) {
        Worker worker = other.gameObject.GetComponent<Worker>();
        if (worker && currentTarget == null && worker.GetComponent<PlacedItem>() && isUncoveredByClouds) {
            myAnimator.SetBool("isAttacking", true);
            myAnimator.Play("Attack", -1, Random.Range(0f,1f));
            currentTarget = worker;
            AudioManager.instance.Play("Orc Attack");
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.GetComponent<Worker>() == currentTarget) {
            myAnimator.SetBool("isAttacking", false);
            currentTarget = null;
        }
    }

    public void ShootArrow() {
        if (currentTarget == null) { return; }

        GameObject newArrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        newArrow.GetComponent<Arrow>().UpdateCurrentTarget(currentTarget);
        AudioManager.instance.Play("Arrow Launch");
    }
}
