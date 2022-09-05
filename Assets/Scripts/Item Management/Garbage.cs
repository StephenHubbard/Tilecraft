using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    Animator myAnimator;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    public void GarbageSpriteOpen() {
        myAnimator.SetBool("HoverOver", true);
    }

    public void GarbageSpriteClosed() {
        myAnimator.SetBool("HoverOver", false);
    }

    public void PlayGarbageOpenSound() {
        AudioManager.instance.Play("Garbage Open");
    }
}
