using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [SerializeField] private Transform handleParent;
    [SerializeField] private float timeLeftInDay;

    [Header("How long a day is in seconds")]
    [SerializeField] private float howLongIsOneDay;

    private void Start() {
        timeLeftInDay = howLongIsOneDay;
    }

    private void Update() {
        timeLeftInDay -= Time.deltaTime;

        handleParent.Rotate(new Vector3(0, 0, -15) * Time.deltaTime) ;
    }


}
