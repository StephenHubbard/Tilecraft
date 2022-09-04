using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningCoin : MonoBehaviour
{

    private void Start() {
        EconomyManager.instance.FarmXP(1);
    }

    public void DestroySelf() {
        Destroy(transform.parent.gameObject);
    }
}
