using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcRelic : MonoBehaviour
{
    [SerializeField] private Transform[] orcSpawnPoints;
    [SerializeField] private GameObject orcPrefab;

    private void Start() {
        SpawnOrcs();
    }

    private void SpawnOrcs() {
        int howManyOrcsToSpawn = Random.Range(1, 4);

        for (int i = 0; i <= howManyOrcsToSpawn; i++)
        {
            foreach (var spawnPoint in orcSpawnPoints)
            {
                if (spawnPoint.childCount == 0 && howManyOrcsToSpawn > 0) {
                    GameObject newOrc = Instantiate(orcPrefab, spawnPoint.transform.position, transform.rotation);
                    howManyOrcsToSpawn--;
                }
            }
        }
    }
}
