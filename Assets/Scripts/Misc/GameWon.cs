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

        float totalDays = TimeOfDay.instance.totalTimeElapsed / TimeOfDay.instance.howLongIsOneDay;
        string totalDaysString = totalDays.ToString().Substring(0, 4);
        float fractionOfADay = (TimeOfDay.instance.totalTimeElapsed / TimeOfDay.instance.howLongIsOneDay);
        string fractionOfADayString = (fractionOfADay.ToString()).Substring(2, 2);


        timeToWinText.text = "You won in: " + totalDaysString + " days!";
        
        Time.timeScale = 0;
    }

}
