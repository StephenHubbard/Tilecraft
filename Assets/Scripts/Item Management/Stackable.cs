using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Rendering;

public class Stackable : MonoBehaviour
{
    public Transform potentialParentItem;
    public int amountOfChildItems = 0;

    public ItemInfo itemInfo;
    public Transform lastChild;

    private AudioManager audioManager;
    private int interactableLayerMask;

    public bool isSellable = false;

    private void Awake() {
        audioManager = FindObjectOfType<AudioManager>();
        itemInfo = GetComponent<DraggableItem>().itemInfo;
        interactableLayerMask = LayerMask.GetMask("Interactable");
    }

    private void Start() {
        FindNearbySameQOL(false);
        FindAmountOfChildren(this.transform);
        StartCoroutine(CanSellCo());
    }

    private void OnMouseExit() {
        transform.GetChild(0).GetComponent<SortingGroup>().sortingOrder = 0;
    }

    private IEnumerator CanSellCo() {
        yield return new WaitForEndOfFrame();

        if (itemInfo.itemName == "Explorer Pendant" || itemInfo.itemName == "Warrior Pendant" ||itemInfo.itemName == "Tech Pendant") {
            isSellable = false;
        } else {
            isSellable = true;
        }
    }

    public void AttachToParent(bool playClickSound) {

        FindLastChild(potentialParentItem);

        gameObject.transform.SetParent(lastChild, true);

        Vector3 newPos = lastChild.transform.position + new Vector3(0, -.4f, 0f);
        gameObject.transform.position = newPos;

        if (playClickSound) {
            audioManager.Play("Stack");
        }
    }

    public void FindLastChild(Transform thisChild) {
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
        transform.SetParent(null);
        
    }

    public void FindNearbySameQOL(bool playClickSound) {
        Collider2D[] allNearbyItems = Physics2D.OverlapCircleAll(transform.position, 1.8f, interactableLayerMask);

        foreach (var item in allNearbyItems)
        {
            if (item.GetComponent<DraggableItem>().itemInfo == transform.GetComponent<DraggableItem>().itemInfo && item.transform != this.transform) {

                if (item.transform.childCount > 1) {
                    potentialParentItem = item.transform.root;
                    AttachToParent(playClickSound);
                    return;
                } else {
                    potentialParentItem = item.transform;
                    AttachToParent(playClickSound);
                    return;
                }
            }
        }
    }
    
}
