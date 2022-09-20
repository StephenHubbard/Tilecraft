using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadClouds : MonoBehaviour
{
    [SerializeField] private GameObject[] cloudPrefabs;

    public static LoadClouds instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnClouds(GameData data) {
        foreach (KeyValuePair<string, Vector3> kvp in data.cloudPositions) {
            Vector3 cloudPos;
            data.cloudPositions.TryGetValue(kvp.Key, out cloudPos);

            int randomNum = Random.Range(0, 2);
            GameObject loadedCloud = Instantiate(cloudPrefabs[randomNum], cloudPos, transform.rotation);
        }

    }
}
