using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [SerializeField] private Transform handleParent;
    [SerializeField] private float timeLeftInDay;

    [Header("How long a day is in seconds")]
    [SerializeField] private float howLongIsOneDay;
    
    private float clockSpinFactor;

    private FoodManager foodManager;

    private void Awake() {
        foodManager = FindObjectOfType<FoodManager>();
    }

    private void Start() {
        clockSpinFactor = 360f / howLongIsOneDay ;
        timeLeftInDay = howLongIsOneDay;
    }

    private void Update() {
        timeLeftInDay -= Time.deltaTime;

        handleParent.transform.eulerAngles = new Vector3(0, 0, timeLeftInDay * clockSpinFactor);

        ResetDay();
    }

    private void ResetDay() {
        if (timeLeftInDay <= 0) {
            timeLeftInDay = howLongIsOneDay;

            foodManager.NewDayEatFood();
        }
    }


}
