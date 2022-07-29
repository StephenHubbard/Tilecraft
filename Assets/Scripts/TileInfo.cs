using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile", menuName = "Create New Tile")]
public class TileInfo : ScriptableObject
{
    public new string name;
    public GameObject tilePrefab;
}
