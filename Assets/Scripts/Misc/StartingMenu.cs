using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour
{
    [SerializeField] private GameObject worldSpaceCavas;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject startingMenuCanvas;
    
    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start() {
        InputManager.instance.isOnMainMenu = true;
    }

    public void NewGameButton() {
        AudioManager.instance.Play("UI Click");
        AudioManager.instance.Play("Wind");
        myAnimator.SetTrigger("Start Game");
        NewGameStartLogic();
    }

    public void LoadGameButton() {
        AudioManager.instance.Play("UI Click");
        SaveLoadGame.instance.LoadGameLogic();
    }

    public void OptionsButton() {
        AudioManager.instance.Play("UI Click");
    }

    public void ExitGameButon() {
        AudioManager.instance.Play("UI Click");
        Application.Quit();
    }

    private void NewGameStartLogic() {
        AudioManager.instance.Play("UI Click");
        InputManager.instance.isOnMainMenu = false;
        StartCoroutine(startingMenuInactiveCo());
    }

    private IEnumerator startingMenuInactiveCo() {
        yield return new WaitForSeconds(2f);
        HousingManager.instance.SpawnStartingThreeWorkers();
        worldSpaceCavas.SetActive(true);
        uiCanvas.SetActive(true);
        startingMenuCanvas.SetActive(false);
    }

}
