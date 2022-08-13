using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningCoin : MonoBehaviour
{
    public void DestroySelf() {
        Destroy(transform.parent.gameObject);
    }
}
