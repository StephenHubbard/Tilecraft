using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SellButton : MonoBehaviour
{
    public bool sellableItemActive = false;
    public bool overSellBox = false;
    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    public void OnHoverEnter() {
        overSellBox = true;
        myAnimator.SetBool("overSellBox", true);
    }

    public void OnHoverExit() {
        overSellBox = false;
        myAnimator.SetBool("overSellBox", false);
    }
}