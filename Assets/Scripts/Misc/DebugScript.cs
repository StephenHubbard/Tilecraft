using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class DebugScript : MonoBehaviour
{
    [SerializeField] private GameObject workerPrefab;

    private bool spedUp = false;

    public void SpawnWorkerButton() {
        Vector2 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        Instantiate(workerPrefab, spawnPosition, transform.rotation);
    }

    public void SpeedUpTime() {
        if (spedUp == false) {
            spedUp = true;
            Time.timeScale = 3;
        } else {
            spedUp = false;
            Time.timeScale = 1;
        }
    }
}
