using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Transform currentTarget = null;

    

    private void Update() {
        if (currentTarget) {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, 3f * Time.deltaTime);

            if (Vector3.Distance(transform.position, currentTarget.transform.position) < .1f) {
                if (currentTarget.GetComponent<Worker>()) {
                    currentTarget.GetComponent<Worker>().TakeDamage(1, null);
                    AudioManager.instance.Play("Arrow Hit");
                }

                if (currentTarget.GetComponent<Knight>()) {
                    currentTarget.GetComponent<Knight>().TakeDamage(1, null);
                    AudioManager.instance.Play("Arrow Hit");
                }

                if (currentTarget.GetComponent<Archer>()) {
                    currentTarget.GetComponent<Archer>().TakeDamage(1, null);
                    AudioManager.instance.Play("Arrow Hit");
                }
                Destroy(gameObject);
            }
        } else {
            Destroy(gameObject);
        }
    }

    public void UpdateCurrentTarget(Transform target) {
        currentTarget = target;

        transform.LookAt(currentTarget.transform.position);
        transform.right = currentTarget.transform.position - transform.position;
    }
}
