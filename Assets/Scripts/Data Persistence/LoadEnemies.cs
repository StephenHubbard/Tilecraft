using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEnemies : MonoBehaviour
{
    [SerializeField] private GameObject skeletonWarrior;
    [SerializeField] private GameObject orcArcherPrefab;

    public static LoadEnemies instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnEnemies(GameData data) {
        foreach (KeyValuePair<string, bool> kvp in data.isSkeletonWarrior)
        {
            GameObject spawnedEnemy;

            Vector3 enemyPos;
            data.enemyPos.TryGetValue(kvp.Key, out enemyPos);

            bool isSkeletonWarrior;
            data.isSkeletonWarrior.TryGetValue(kvp.Key, out isSkeletonWarrior);
            if (isSkeletonWarrior) {
                spawnedEnemy = Instantiate(skeletonWarrior, enemyPos, transform.rotation);
            } else {
                spawnedEnemy = Instantiate(orcArcherPrefab, enemyPos, transform.rotation);
            }

            RaycastHit2D[] hitArray = Physics2D.RaycastAll(spawnedEnemy.transform.position, Vector2.zero, 100f);

            Tile parentTile = null;
            Transform parentTransform = null;

            foreach (var item in hitArray)
            {
                if (item.transform.GetComponent<Tile>()) {
                    parentTile = item.transform.GetComponent<Tile>();
                    break;
                }
            }

            foreach (var orcSpawnPoint in parentTile.GetComponentInChildren<OrcRelic>().orcSpawnPoints)
            {
                if (orcSpawnPoint.transform.childCount == 0) {
                    parentTransform = orcSpawnPoint;
                    spawnedEnemy.transform.SetParent(parentTransform);
                    spawnedEnemy.transform.position = parentTransform.position;
                    break;
                }
            }
        }

    }
}
