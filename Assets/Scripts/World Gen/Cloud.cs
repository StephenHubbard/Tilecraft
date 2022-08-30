using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Color spriteRendererColor;
    private Animator myAnimator;
    private Collider2D myCollider;

    private void Awake() {
        myCollider = GetComponent<CapsuleCollider2D>();
        spriteRendererColor = GetComponent<SpriteRenderer>().color;
        myAnimator = GetComponent<Animator>();
    }
    
    public void FadeDestroyCloud() {
        StartCoroutine(DestroyCloudCo());
    }

    private IEnumerator DestroyCloudCo() {
        myAnimator.SetTrigger("Fade");
        myCollider.enabled = false;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}
