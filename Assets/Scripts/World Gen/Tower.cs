using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Cloud>()) {
            other.gameObject.GetComponent<Cloud>().FadeDestroyCloud();
        }
    }
}
