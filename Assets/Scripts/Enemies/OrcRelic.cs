using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcRelic : MonoBehaviour
{
    [SerializeField] public Transform[] orcSpawnPoints;
    [SerializeField] private GameObject orcPrefab;

    public bool hasEnemies = true;

    private void Start() {
        SpawnOrcs();
    }

    private void SpawnOrcs() {
        int howManyOrcsToSpawn = Random.Range(1, 3);

        for (int i = 0; i <= howManyOrcsToSpawn; i++)
        {
            foreach (var spawnPoint in orcSpawnPoints)
            {
                if (spawnPoint.childCount == 0 && howManyOrcsToSpawn > 0) {
                    GameObject newOrc = Instantiate(orcPrefab, spawnPoint.transform.position, transform.rotation);
                    newOrc.transform.SetParent(spawnPoint);
                    howManyOrcsToSpawn--;
                }
            }
        }
    }

    public IEnumerator DetectEnemiesCo() {
        yield return new WaitForEndOfFrame();

        int potentialEnemies = 3;

        foreach (var spawnPoint in orcSpawnPoints)
        {
            if (spawnPoint.childCount == 0) {
                potentialEnemies--;
            } 
        }

        if (potentialEnemies > 0) {
            hasEnemies = true;
        } else {
            hasEnemies = false;
            GetComponentInParent<CraftingManager>().CheckCanStartCrafting();
        }
    }

    public void DetectIfEnemies() {
        StartCoroutine(DetectEnemiesCo());
    }
}
