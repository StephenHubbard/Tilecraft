using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private GameObject gameWinContainer;

    private void Update() {
        OpenMenu();
    }

    private void OpenMenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Time.timeScale = 0;
            menuContainer.SetActive(true);
        }
    }

    public void MainMenuButton() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SaveGameButton() {

        SaveLoadGame.instance.SaveGameLogic();
    }

    public void ResumeButton() {
        Time.timeScale = 1;
        menuContainer.SetActive(false);
        gameWinContainer.SetActive(false);
    }

    public void ExitGameButton() {
        Application.Quit();
    }
}
