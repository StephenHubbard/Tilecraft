using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    private Transform currentTarget = null;
    public Transform archerThatLaunchedArrow = null;
    [SerializeField] private GameObject chinkVFXPrefab;

    private void Update() {
        if (currentTarget) {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, 3f * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget.transform.position) < .1f) {

                if (currentTarget.GetComponent<Enemy>().isMaskedOrc) {
                    Instantiate(chinkVFXPrefab, transform.position, transform.rotation);
                    AudioManager.instance.Play("Armor Chink");

                    if (Vector3.Distance(transform.position, archerThatLaunchedArrow.transform.position) < 1f) {
                        currentTarget.GetComponent<Enemy>().currentTarget = archerThatLaunchedArrow;
                        currentTarget.GetComponent<Enemy>().myAnimator.SetBool("isAttacking", true);
                        currentTarget.GetComponent<Enemy>().clubSprite.SetActive(true);
                    }

                } else {
                    if (currentTarget != null) {
                        currentTarget.GetComponent<Enemy>().TakeDamage(1, archerThatLaunchedArrow);
                    }
                }
            
                Destroy(gameObject);
            }
        } else {
            archerThatLaunchedArrow.GetComponent<Archer>().currentTarget = null;
            Destroy(gameObject);
        }
    }

    public void UpdateCurrentTarget(Transform target) {
        currentTarget = target;

        transform.LookAt(currentTarget.transform.position);
        transform.right = currentTarget.transform.position - transform.position;
    }
}
