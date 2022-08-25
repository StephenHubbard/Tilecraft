using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewItemManager : MonoBehaviour
{
    public static NewItemManager instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnNewItem(Vector3 spawnPostion, ItemInfo itemInfo){
        StartCoroutine(SpawnNewItemCo(spawnPostion, itemInfo));
    }

    private IEnumerator SpawnNewItemCo(Vector3 spawnPostion, ItemInfo itemInfo) {
        yield return new WaitForEndOfFrame();

        GameObject newItem = Instantiate(itemInfo.draggableItemPrefab, spawnPostion, transform.rotation);
        newItem.name = "test object";
        AudioManager.instance.Play("Pop");
    }
}
