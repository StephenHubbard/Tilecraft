using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class PackManager : MonoBehaviour
{
    [SerializeField] private GameObject[] resourcePackItems;

    public void SpawnResourcePack() {
        Vector3 packSpawnLocation = UtilsClass.GetMouseWorldPosition() + new Vector3(Random.Range(-3f, -5f), Random.Range(-3f, -5f), 0);

        Instantiate(resourcePackItems[Random.Range(0, resourcePackItems.Length)], packSpawnLocation, transform.rotation);
    }
}
