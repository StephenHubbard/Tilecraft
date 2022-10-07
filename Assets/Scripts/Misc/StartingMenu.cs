using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartingMenu : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject worldSpaceCavas;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject startingMenuCanvas;
    [SerializeField] private GameObject optionsContainer;
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private TMP_Text resText;
    [SerializeField] private GameObject logo;
    [SerializeField] private Toggle fullScreenTog;
    [SerializeField] private List<ResItem> resolutions = new List<ResItem>();
    public int selectedResolution = 0;
    
    private Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start() {
        InputManager.instance.isOnMainMenu = true;

        if (fullScreenTog.isOn) {
            Screen.fullScreen = true;
        }

        DetectCurrentRes();
    }

    public void UpdateMusicVolume() {
        AudioListener.volume = musicSlider.value;
    }

    public void UpdateSFXVolume() {
        AudioListener.volume = sfxSlider.value;
    }

    private void DetectCurrentRes() {
        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical) {
                foundRes = true;
                selectedResolution = i;
                UpdateResText();
            }
        }

        if (!foundRes) {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;
            resolutions.Add(newRes);
            selectedResolution = resolutions.Count - 1;
            UpdateResText();
        }
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
        optionsContainer.SetActive(true);
        mainMenuContainer.SetActive(false);
        logo.SetActive(false);
    }

    public void SaveOptionsButton() {
        AudioManager.instance.Play("UI Click");
        if (fullScreenTog.isOn) {
            Screen.fullScreen = false;
        } else {
            Screen.fullScreen = true;
        }
            
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullScreenTog.isOn);
    }

    public void BackOptionsButton() {
        AudioManager.instance.Play("UI Click");
        optionsContainer.SetActive(false);
        mainMenuContainer.SetActive(true);
        logo.SetActive(true);
    }

    public void UpdateResText() {
        resText.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    public void ResLeft() {
        selectedResolution--;
        if (selectedResolution < 0) {
            selectedResolution = 0;
        }
        UpdateResText();
    }

    public void ResRight() {
        selectedResolution++;
        if (selectedResolution > resolutions.Count - 1) {
            selectedResolution = resolutions.Count - 1;
        }
        UpdateResText();
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

    [System.Serializable]
    public class ResItem {
        public int horizontal, vertical;
    }

}
