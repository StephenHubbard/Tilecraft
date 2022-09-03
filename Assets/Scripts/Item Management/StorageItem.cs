using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class StorageItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text amountText;

    public ItemInfo itemInfo;

    public int amountInStorage = 0;

    private void Start() {
        itemInfo = GetComponent<UITooltip>().itemInfo;
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
                Instantiate(itemInfo.draggableItemPrefab, transform.position + new Vector3(0, 0, -90f), transform.rotation);
                AudioManager.instance.Play("Pop");
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left) {
            amountInStorage--;
            Instantiate(itemInfo.draggableItemPrefab, transform.position + new Vector3(0, -1.5f, -90f), transform.rotation);
            AudioManager.instance.Play("Pop");
        }

        CheckIfZero();

    }

    private void CheckIfZero() {
        if (amountInStorage == 0) {
            Destroy(gameObject);
        }
    }
}
