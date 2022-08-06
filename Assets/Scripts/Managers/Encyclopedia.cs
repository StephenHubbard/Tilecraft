using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encyclopedia : MonoBehaviour
{
    [SerializeField] private GameObject encylopediaContainer;

    public void OpenEncylopedia() {
        encylopediaContainer.SetActive(true);
    }

    public void CloseEncylopedia() {
        encylopediaContainer.SetActive(false);
    }
}
