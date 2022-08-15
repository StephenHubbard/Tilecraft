using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponType {
        pitchfork, 
        sword, 
        bow
    }

    public WeaponType weaponType;
}
