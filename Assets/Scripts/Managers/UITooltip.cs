using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITooltip : MonoBehaviour
{
    [SerializeField] public string toolTipName;

    [TextArea]
    [SerializeField] public string toolTipText;

    private EconomyManager economyManager;

    private void Awake() {
        economyManager = FindObjectOfType<EconomyManager>();
    }

    private void Update() {
        if (toolTipName == "Coins") {
            toolTipText = "You currently have " + economyManager.currentCoins + " coins.";
        }
    }
}
