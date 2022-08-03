using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Stackable : MonoBehaviour
{
    public Transform potentialParentItem;
    public int amountOfChildItems = 0;

    // public bool isInStackAlready = false;
    public ItemInfo itemInfo;
    private Transform lastChild;

    private AudioManager audioManager;
    private int interactableLayerMask;

    private void Awake() {
        audioManager = FindObjectOfType<AudioManager>();
        itemInfo = GetComponent<DraggableItem>().itemInfo;
        interactableLayerMask = LayerMask.GetMask("Interactable");
    }

    // private void OnMouseDrag() {
    //     RaycastHit2D[] hit = Physics2D.RaycastAll(UtilsClass.GetMouseWorldPosition(), Vector2.zero, 100f, interactableLayerMask);

    //     if (hit.Length > 1) {
    //         if (hit[1].transform.gameObject.GetComponent<Stackable>() && hit[1].transform.gameObject.GetComponent<DraggableItem>().itemInfo == itemInfo) {
    //             if (hit[1].transform.root != transform) {
    //                 potentialParentItem = hit[1].transform.root;
    //             }
    //         }
    //     } else {
    //         potentialParentItem = null;
    //     }
    // }

    
    public void AttachToParent() {

        FindLastChild(potentialParentItem);

        gameObject.transform.SetParent(lastChild, true);

        Vector3 newPos = lastChild.transform.position + new Vector3(0, -.4f, 0f);
        gameObject.transform.position = newPos;

        audioManager.Play("Stack");

        // isInStackAlready = true;
        // lastChild.GetComponent<Stackable>().isInStackAlready = true;
        
        // Stackable[] allItems = FindObjectsOfType<Stackable>(); 

        // foreach (var item in allItems)
        // {
        //     if (item.isInStackAlready) {
        //         item.potentialParentItem = null;
        //     }
        // }
        
    }

    private void FindLastChild(Transform thisChild) {
        if (thisChild.childCount < 2) {
            lastChild = thisChild;
        } else {
            FindLastChild(thisChild.GetChild(1));
        }
    }

    public void FindAmountOfChildren(Transform parentItem) {
        amountOfChildItems = 0;

        Stackable[] amountOfChildren = GetComponentsInChildren<Stackable>();

        foreach (var item in amountOfChildren)
        {
            amountOfChildItems += 1;
        }
    }

    public void DetachFromParent() {
        // isInStackAlready = false;
        transform.SetParent(null);
        // transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        
    }
    
}
