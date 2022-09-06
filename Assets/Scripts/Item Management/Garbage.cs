using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour
{
    Animator myAnimator;

    private Coroutine garbageOpenCo;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    public void GarbageSpriteOpen() {
        if (garbageOpenCo == null) {
            garbageOpenCo = StartCoroutine(OpenGarbageCanDelayCo());
        }
    }

    public void GarbageSpriteClosed() {
        myAnimator.SetBool("HoverOver", false);
        StopAllCoroutines();
        garbageOpenCo = null;
    }

    public void PlayGarbageOpenSound() {
        AudioManager.instance.Play("Garbage Open");
    }

    private IEnumerator OpenGarbageCanDelayCo() {
        float duration = .15f; 
        float normalizedTime = 0;
        while(normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }

        myAnimator.SetBool("HoverOver", true);
    }
}
