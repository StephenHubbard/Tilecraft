using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stackable : MonoBehaviour
{
    public Transform potentialParentItem;

    public bool isInStackAlready = false;
    private ItemInfo itemInfo;
    private Transform lastChild;

    private void Awake() {
        itemInfo = GetComponent<DraggableItem>().itemInfo;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.GetComponent<Stackable>() && other.gameObject.GetComponent<DraggableItem>().itemInfo == itemInfo && !isInStackAlready) {
            potentialParentItem = other.gameObject.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.transform == potentialParentItem) {
            potentialParentItem = null;
        }
    }

    public void AttachToParent() {

        FindLastChild(potentialParentItem);

        gameObject.transform.parent = lastChild;

        Vector3 newPos = lastChild.transform.position + new Vector3(0, -.4f, 0);

        gameObject.transform.position = newPos;

        isInStackAlready = true;
        lastChild.GetComponent<Stackable>().isInStackAlready = true;
        
        Stackable[] allItems = FindObjectsOfType<Stackable>(); 

        foreach (var item in allItems)
        {
            if (item.isInStackAlready) {
                item.potentialParentItem = null;
            }
        }
    }

    private void FindLastChild(Transform thisChild) {
        if (thisChild.childCount < 2) {
            lastChild = thisChild;
        } else {
            FindLastChild(thisChild.GetChild(1));
        }
    }

    public void DetachFromParent() {
        isInStackAlready = false;
        transform.SetParent(null);
    }
    
}
