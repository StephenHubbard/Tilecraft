using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentXP;
    public Vector3 tilePositionTest;
    // public SerializableDictionary<string, Vector3> tilePositions;
    // public List<string> allTilesGuid;

    public GameData() {
        this.currentXP = 0;
        this.tilePositionTest = new Vector3();
        // tilePositions = new SerializableDictionary<string, Vector3>();
        // allTilesGuid = new List<string>();
    }
}
