using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menuContainer;

    private void Update() {
        OpenMenu();
    }

    private void OpenMenu() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Time.timeScale = 0;
            menuContainer.SetActive(true);
        }
    }

    public void QuitButton() {
        Application.Quit();
    }

    public void RestartButton() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ResumeButton() {
        Time.timeScale = 1;
        menuContainer.SetActive(false);
    }
}
