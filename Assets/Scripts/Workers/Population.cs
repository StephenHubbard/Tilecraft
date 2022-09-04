using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    [SerializeField] public int currentLevel = 0;
    [SerializeField] private Transform starContainer;
    [SerializeField] private GameObject circleHighlight;

    public bool isMaxLevel = false;

    public void UpLevelStars(bool increaseLevel) {
        if (increaseLevel) {
            currentLevel++;
        }

        if (starContainer != null) {
            if (currentLevel == 1) {
                starContainer.GetChild(0).gameObject.SetActive(true);
            }

            if (currentLevel == 2) {
                starContainer.GetChild(0).gameObject.SetActive(false);
                starContainer.GetChild(1).gameObject.SetActive(true);
            }

            if (currentLevel == 3) {
                starContainer.GetChild(1).gameObject.SetActive(false);
                starContainer.GetChild(2).gameObject.SetActive(true);
                isMaxLevel = true;
            }
        }
    }

    public void ToggleCircleHighlightOn() {
        if (circleHighlight != null) {
            circleHighlight.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void ToggleCircleHighlightOff() {
        if (circleHighlight != null) {
            circleHighlight.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void TransferLevel(int currentLevel) {
        this.currentLevel = currentLevel;
        UpLevelStars(false);
    }


}
