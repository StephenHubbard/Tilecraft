using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class DebugScript : MonoBehaviour
{
    [SerializeField] private GameObject workerPrefab;

    public void SpawnWorkerButton() {
        Vector2 spawnPosition = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        Instantiate(workerPrefab, spawnPosition, transform.rotation);
    }
}
