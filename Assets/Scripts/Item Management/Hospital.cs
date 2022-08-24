using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hospital : MonoBehaviour
{

    [SerializeField] private float healTime = 20f;

    public void HealPerson(GameObject person) {

        GetComponentInParent<CraftingManager>().sliderCanvas.SetActive(true);
        GetComponentInParent<CraftingManager>().tileSlider.maxValue = healTime;
        GetComponentInParent<CraftingManager>().tileSlider.value = healTime;
        KillCoroutines();
        StartCoroutine(HealCo(person));
    }

    private IEnumerator HealCo(GameObject person) {
        bool doneHealing = false;
        Slider slider = GetComponentInParent<CraftingManager>().tileSlider;

        while (!doneHealing) {
            slider.value -= Time.deltaTime;

            if (slider.value < .1f) {
                doneHealing = true;

                if (person.gameObject.GetComponent<Worker>()) {
                    person.gameObject.GetComponent<Worker>().myHealth = 3;
                }

                if (person.gameObject.GetComponent<Archer>()) {
                    person.gameObject.GetComponent<Archer>().myHealth = 4;
                }

                if (person.gameObject.GetComponent<Knight>()) {
                    person.gameObject.GetComponent<Knight>().myHealth = 6;
                }
            }

            yield return null;

        }

        GetComponentInParent<CraftingManager>().sliderCanvas.SetActive(false);
    }

    public void KillCoroutines() {
        StopAllCoroutines();
    }
}
