using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuContainer;
    [SerializeField] private GameObject gameWinContainer;
    [SerializeField] private GameObject gameSavedText;

    public bool isPaused;

    public static Menu instance;

    private void Awake() {
        instance = this;
    }

    private void Update() {
        OpenMenu();
    }

    private void OpenMenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            AudioManager.instance.Play("UI Click");
            Time.timeScale = 0;
            menuContainer.SetActive(true);
            isPaused = true;
        }
    }

    public void MainMenuButton() {
        AudioManager.instance.Play("UI Click");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SaveGameButton() {
        AudioManager.instance.Play("UI Click");
        DataPersistenceManager.instance.SaveGame();
        gameSavedText.SetActive(true);
        PlayerPrefs.SetInt("SavedGame", 1);
    }


    public void ResumeButton() {
        gameSavedText.SetActive(false);
        AudioManager.instance.Play("UI Click");
        ToolTipManager.instance.isOverUI = false;
        isPaused = false;
        Time.timeScale = 1;
        menuContainer.SetActive(false);
        gameWinContainer.SetActive(false);
    }

    public void ExitGameButton() {
        AudioManager.instance.Play("UI Click");
        Application.Quit();
    }
}
