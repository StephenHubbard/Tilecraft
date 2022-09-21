using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAnim : MonoBehaviour
{
    private Vector2 targetPop;

    private void Start() {
        // targetPop = new Vector2(0f, 0f);
    }

    public void FeedPopulation(Population population) {
        targetPop = population.transform.position;
        StartCoroutine(SelfDestroy(population));
    }

    private void Update() {
        if (targetPop != null) {
            transform.position = Vector2.MoveTowards(transform.position, targetPop, Time.deltaTime * 2);
        }
    }

    private IEnumerator SelfDestroy(Population population) {
        yield return new WaitForSeconds(.5f);
        if (population.GetComponent<Worker>()) {
            population.GetComponent<Worker>().FeedWorker(1, true);
        }
        if (population.GetComponent<Archer>()) {
            population.GetComponent<Archer>().FeedArcher(1, true);
        }
        if (population.GetComponent<Knight>()) {
            population.GetComponent<Knight>().FeedKnight(1, true);
        }
        Destroy(this.gameObject);
    }
}
