using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public bool showTutorial = true;
    [SerializeField] private GameObject tutorialContainer;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject closeButton;

    public int tutorialIndexNum = 0;

    [TextArea]
    [SerializeField] private string[] tutorialStrings;

    public static TutorialManager instance;

    private Animator myAnimator;


    private void Awake() {
        instance = this;

        myAnimator = tutorialContainer.GetComponent<Animator>();
    }

    private void Start() {
        if (showTutorial) {
            tutorialContainer.SetActive(true);
        }

        ActivateNextTutorial();
    }

    public void MoveBoxToLeft() {
        myAnimator.SetTrigger("MoveLeft");
    }

    public void MoveBoxToBottom() {
        myAnimator.SetTrigger("MoveDown");
    }

    public void ActivateNextTutorial() {
        if (!showTutorial) { return; }

        tutorialText.text = tutorialStrings[tutorialIndexNum];
        tutorialContainer.SetActive(true);
    }

    public void ShowCloseButton() {
        closeButton.SetActive(true);
    }

    public void CompleteThisSectionOfTutorial() {
        AudioManager.instance.Play("UI Click");
        tutorialContainer.SetActive(false);
    }

}
