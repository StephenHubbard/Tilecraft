using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadGame : MonoBehaviour
{
    public static SaveLoadGame instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
    }

    public void SaveGameLogic() {

        print("game saved");
    }

    public void LoadGameLogic() {

        print("loaded game");
    }
}
