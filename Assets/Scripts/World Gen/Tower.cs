using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    [SerializeField] private Color pastelRed = new Color();

    private void Start() {
        Color newRed = new Color(pastelRed.r, pastelRed.g, pastelRed.b);
        GetComponentInParent<CraftingManager>().sliderBackgroundColor.color = newRed;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Cloud>()) {
            other.gameObject.GetComponent<Cloud>().FadeDestroyCloud();
        }
    }

    private void OnDestroy() {
        GetComponentInParent<CraftingManager>().sliderBackgroundColor.color = GetComponentInParent<CraftingManager>().defaultGreen;
    }
}
