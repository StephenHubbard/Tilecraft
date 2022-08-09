using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TierOneCollider : MonoBehaviour
{
    private void Start() {
        StartCoroutine(DestroyObject());
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<OrcRelic>()) {
            other.gameObject.transform.parent.GetComponent<Tile>().isOccupiedWithBuilding = false;
            Destroy(other.gameObject);
        }
    }

    private IEnumerator DestroyObject() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
