using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hospital : MonoBehaviour
{

    [SerializeField] private float healTime = 20f;
    [SerializeField] private GameObject healVFXprefab;

    public bool isHealing = false;

    public void HealPerson(GameObject person) {

        GetComponentInParent<CraftingManager>().sliderCanvas.SetActive(true);
        GetComponentInParent<CraftingManager>().tileSlider.maxValue = healTime;
        GetComponentInParent<CraftingManager>().tileSlider.value = healTime;
        KillCoroutines();
        StartCoroutine(HealCo(person));
    }

    private IEnumerator HealCo(GameObject person) {
        isHealing = true;
        bool doneHealing = false;
        Slider slider = GetComponentInParent<CraftingManager>().tileSlider;

        while (!doneHealing) {
            slider.value -= Time.deltaTime;

            if (slider.value < .1f) {
                doneHealing = true;
                isHealing = false;

                Transform[] workerPointsInTile = GetComponentInParent<Tile>().workerPoints;

                foreach (Transform item in workerPointsInTile)
                {
                    if (item.childCount > 0) {
                        if (item.transform.GetChild(0).GetComponent<Worker>()) {
                            item.transform.GetChild(0).GetComponent<Worker>().myHealth = item.transform.GetChild(0).GetComponent<Worker>().maxHealth;
                        }

                        if (item.transform.GetChild(0).GetComponent<Knight>()) {
                            item.transform.GetChild(0).GetComponent<Knight>().myHealth = item.transform.GetChild(0).GetComponent<Knight>().maxHealth;
                        }

                        if (item.transform.GetChild(0).GetComponent<Archer>()) {
                            item.transform.GetChild(0).GetComponent<Archer>().myHealth = item.transform.GetChild(0).GetComponent<Archer>().maxHealth;
                        }
                    }
                }
                
            }

            yield return null;

        }

        AudioManager.instance.Play("Heal Complete");
        Instantiate(healVFXprefab, transform.position, transform.rotation);
        GetComponentInParent<CraftingManager>().sliderCanvas.SetActive(false);
    }

    public void KillCoroutines() {
        StopAllCoroutines();
    }
}
