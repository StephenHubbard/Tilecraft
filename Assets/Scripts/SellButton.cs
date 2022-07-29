using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SellButton : MonoBehaviour
{
    public bool sellableItemActive = false;
    public bool overSellBox = false;

    public void OnHoverEnter() {
        overSellBox = true;
    }

    public void OnHoverExit() {
        overSellBox = false;

    }
}