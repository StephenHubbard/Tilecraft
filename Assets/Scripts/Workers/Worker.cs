using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start() {
        StartCoroutine(FindObjectOfType<FoodManager>().UpdateFoodNeededCo());

        if (GetComponent<PlacedItem>()) {
            AnimatorStateInfo state = myAnimator.GetCurrentAnimatorStateInfo (0);
            myAnimator.Play (state.fullPathHash, -1, Random.Range(0f,1f));
        }
    }

    public void StartWorking() {
        StartCoroutine(StartWorkingCo());
    }

    public void StopWorking() {
        StartCoroutine(StopWorkingCo());
    }

    private IEnumerator StartWorkingCo() {
        yield return new WaitForSeconds(Random.Range(0f, .4f));
        myAnimator.SetBool("isWorking", true);
    }

    private IEnumerator StopWorkingCo() {
        yield return new WaitForSeconds(Random.Range(0f, .4f));
        myAnimator.SetBool("isWorking", false);
    }
}
