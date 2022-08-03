using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Color spriteRendererColor;
    private Animator myAnimator;

    private void Awake() {
        spriteRendererColor = GetComponent<SpriteRenderer>().color;
        myAnimator = GetComponent<Animator>();
    }
    
    public void FadeDestroyCloud() {
        StartCoroutine(DestroyCloudCo());
    }

    private IEnumerator DestroyCloudCo() {
        myAnimator.SetTrigger("Fade");
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
