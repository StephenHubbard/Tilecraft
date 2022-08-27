using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hospital : MonoBehaviour
{

    [SerializeField] private float healTime = 20f;
    [SerializeField] private GameObject healVFXprefab;

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
                    person.gameObject.GetComponent<Worker>().myHealth = person.gameObject.GetComponent<Worker>().maxHealth;
                }

                if (person.gameObject.GetComponent<Archer>()) {
                    person.gameObject.GetComponent<Archer>().myHealth = person.gameObject.GetComponent<Archer>().maxHealth;
                }

                if (person.gameObject.GetComponent<Knight>()) {
                    person.gameObject.GetComponent<Knight>().myHealth = person.gameObject.GetComponent<Knight>().maxHealth;
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
