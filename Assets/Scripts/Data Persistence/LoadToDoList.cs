using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadToDoList : MonoBehaviour
{
    public static LoadToDoList instance;

    private void Awake() {
        instance = this;
    }

    public void SpawnToDoList(GameData data) {
        StartCoroutine(NewToDoListCo(data));
    }

    private IEnumerator NewToDoListCo(GameData data) {
        yield return null;

        ToDoList[] allToDoLists = FindObjectsOfType<ToDoList>();

        foreach (var item in allToDoLists)
        {
            Destroy(item.gameObject);
        }

        ToDoManager.instance.toDoList.Clear();

        foreach (KeyValuePair<string, ItemInfo> kvp in data.toDoList) {
            ItemInfo itemInfo;
            data.toDoList.TryGetValue(kvp.Key, out itemInfo);

            ToDoManager.instance.SetNewToDoList(itemInfo);
        }
    }
}
