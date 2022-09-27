using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour, IDataPersistence
{
    [SerializeField] public string id;

    private Color spriteRendererColor;
    private Animator myAnimator;
    private Collider2D myCollider;

    private void Awake() {
        myCollider = GetComponent<CapsuleCollider2D>();
        spriteRendererColor = GetComponent<SpriteRenderer>().color;
        myAnimator = GetComponent<Animator>();
    }

    private void Start() {
        GenerateGuid();
    }

    public void GenerateGuid() {
        id = System.Guid.NewGuid().ToString();
    }

    public void LoadData(GameData data) {
        
    }

    public void SaveData(ref GameData data) {
        if (data.cloudPositions.ContainsKey(id)) {
            data.cloudPositions.Remove(id);
        }
        data.cloudPositions.Add(id, transform.position);
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
