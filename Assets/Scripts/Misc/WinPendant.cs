using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPendant : MonoBehaviour
{

    private GameWon gameWon;

    private void Awake() {
        gameWon = FindObjectOfType<GameWon>();
    }

    void Start()
    {
        gameWon.GameHasBeenWon();
        Destroy(gameObject);
    }

}
