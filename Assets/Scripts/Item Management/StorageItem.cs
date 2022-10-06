using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using CodeMonkey.Utils;


public class StorageItem : MonoBehaviour, IPointerClickHandler, IDataPersistence, IPointerDownHandler
{
    [SerializeField] public string id;
    [SerializeField] private TMP_Text amountText;

    public ItemInfo itemInfo;

    public int amountInStorage = 0;

    private void Start() {
        GenerateGuid();
        itemInfo = GetComponent<UITooltip>().itemInfo;
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data) {

    }

    public void SaveData(ref GameData data) {
        if (data.storageItemsItemInfo.ContainsKey(id)) {
            data.storageItemsItemInfo.Remove(id);
        }
        data.storageItemsItemInfo.Add(id, itemInfo);
        data.storageItemsAmount.Add(id, amountInStorage);
    }

    private void Update() {
        amountText.text = amountInStorage.ToString();
    }

    public void IncreaseAmount(int amount) {
        amountInStorage += amount;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetKey(KeyCode.LeftShift) && (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)) {
            for (int i = amountInStorage; i > 1; i--)
            {
                amountInStorage--;
                GameObject storageGO = Instantiate(itemInfo.draggableItemPrefab, UtilsClass.GetMouseWorldPosition() + new Vector3(0, 0, 0), transform.rotation);
                AudioManager.instance.Play("Pop");
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right) {
            amountInStorage--;
            Instantiate(itemInfo.draggableItemPrefab, UtilsClass.GetMouseWorldPosition() + new Vector3(0, -1.5f, 0), transform.rotation);
            AudioManager.instance.Play("Pop");
        }

        CheckIfZero();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {
            amountInStorage--;
            GameObject storageGO = Instantiate(itemInfo.draggableItemPrefab, UtilsClass.GetMouseWorldPosition() + new Vector3(0, 0, 0), transform.rotation);
            storageGO.GetComponent<Stackable>().leftClickFromStorage = true;
            AudioManager.instance.Play("Pop");
            storageGO.GetComponent<DragAndDropCustom>().OnMouseDownCustom();
        }

        CheckIfZero();

    }

    private void CheckIfZero() {
        if (amountInStorage == 0) {
            Destroy(gameObject);
        }
    }
}
