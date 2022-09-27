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
        StartCoroutine(loadGameCo());
        TutorialManager.instance.showTutorial = false;
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
        DataPersistenceManager.instance.NewGame();
        StartCoroutine(startingMenuInactiveCo());
    }

    private IEnumerator startingMenuInactiveCo() {
        yield return new WaitForSeconds(2f);
        InputManager.instance.isOnMainMenu = false;
        TutorialManager.instance.StartTutorial();
        HousingManager.instance.SpawnStartingThreeWorkers();
        worldSpaceCavas.SetActive(true);
        uiCanvas.SetActive(true);
        startingMenuCanvas.SetActive(false);
    }

    private IEnumerator loadGameCo() {
        // will implement wind and 2 sec delay after testing
        Tile[] allTiles = FindObjectsOfType<Tile>();
        foreach (var tile in allTiles)
        {
            Destroy(tile.gameObject);
        }

        Cloud[] allClouds = FindObjectsOfType<Cloud>();
        foreach (var cloud in allClouds)
        {
            Destroy(cloud.gameObject);
        }
        yield return new WaitForSeconds(.1f);
        worldSpaceCavas.SetActive(true);
        uiCanvas.SetActive(true);
        startingMenuCanvas.SetActive(false);

        DataPersistenceManager.instance.LoadGame();
        InputManager.instance.isOnMainMenu = false;

    }

}
