using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour
{
    [SerializeField] private GameObject worldSpaceCavas;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject startingMenuCanvas;

    private int testInt;

    private void Start() {
        InputManager.instance.isOnMainMenu = true;
    }

    public void NewGameButton() {
        AudioManager.instance.Play("UI Click");
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
        worldSpaceCavas.SetActive(true);
        uiCanvas.SetActive(true);
        startingMenuCanvas.SetActive(false);
        HousingManager.instance.SpawnStartingThreeWorkers();
        InputManager.instance.isOnMainMenu = false;
    }

}
