using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameWon : MonoBehaviour
{
    [SerializeField] private GameObject GameWonContainer;
    [SerializeField] private TMP_Text timeToWinText;


    public void GameHasBeenWon()
    {
        GameWonContainer.SetActive(true);

        timeToWinText.text = "You won in: " + TimeOfDay.instance.totalTimeElapsed.ToString() + " seconds!";
        
        Time.timeScale = 0;
    }

}
