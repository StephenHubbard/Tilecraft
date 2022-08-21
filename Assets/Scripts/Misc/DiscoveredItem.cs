using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoveredItem : MonoBehaviour
{
    private void Start() {
        AudioManager.instance.Play("New Discovery");
    }

    public void SelfDestroy() {
        Destroy(gameObject);
    }
}
