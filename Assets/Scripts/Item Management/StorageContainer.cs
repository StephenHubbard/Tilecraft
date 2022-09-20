using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class StorageContainer : MonoBehaviour, IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField] public GameObject gridLayoutContainer;
    [SerializeField] private GameObject whiteBorder;
    [SerializeField] private GameObject storageItemPrefab;

    public bool isOverStorage = false;

    public static StorageContainer instance;

    private Animator myAnimator;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        myAnimator = GetComponent<Animator>();
    }

    public void ActivateWhiteBorderOn() {
        whiteBorder.SetActive(true);
    }

    public void ActivateWhiteBorderOff() {
        whiteBorder.SetActive(false);
    }

    public void AddToStorage(ItemInfo itemInfo, int amount) {
        AudioManager.instance.Play("UI Click");
        StorageTutorial();

        if (isAlreadyExistingInStorage(itemInfo)) {
            foreach (Transform item in gridLayoutContainer.transform)
            {
                if (item.GetComponent<UITooltip>().itemInfo == itemInfo) {
                    item.GetComponent<StorageItem>().IncreaseAmount(amount);
                }
            }
        } else {
            GameObject newItem = Instantiate(storageItemPrefab.gameObject, transform.position, transform.rotation);
            newItem.transform.SetParent(gridLayoutContainer.transform);
            newItem.transform.localScale = Vector3.one;
            newItem.GetComponent<Image>().sprite = itemInfo.itemSprite;
            newItem.GetComponent<UITooltip>().itemInfo = itemInfo;
            ActivateWhiteBorderOff();
            newItem.GetComponent<StorageItem>().IncreaseAmount(amount);
            newItem.transform.SetAsFirstSibling();
        }

    }

    private void StorageTutorial() {
        if (TutorialManager.instance.tutorialIndexNum == 4) {
            TutorialManager.instance.tutorialIndexNum++;
            TutorialManager.instance.ActivateNextTutorial();
        }
    }

    public bool CheckIfStorageHasSpace(ItemInfo itemInfo, GameObject go) {

        if (go.GetComponent<Population>() && go.GetComponent<Population>().currentLevel > 0) {
            myAnimator.SetTrigger("Storage Full");
            return false;
        }

        if (isAlreadyExistingInStorage(itemInfo)) {
            return true;
        }

        if (gridLayoutContainer.transform.childCount > 9) {
            myAnimator.SetTrigger("Storage Full");
            return false;
        } 

        return true;
    }

    private bool isAlreadyExistingInStorage(ItemInfo itemInfo) {
        foreach (Transform item in gridLayoutContainer.transform)
        {
            if (item.GetComponent<UITooltip>().itemInfo == itemInfo) {
                return true;
            }
        }

        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOverStorage = true;
    }

    public void OnPointerMove(PointerEventData eventData) {

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOverStorage = false;
    }


}
