using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerGen : MonoBehaviour
{
    [SerializeField] private GameObject flowerPrefab;

    private Collider2D myCollider;

    private void Awake() {
        myCollider = GetComponent<BoxCollider2D>();
    }

    private void Start() {
        CreateFlowers();
    }

    public void CreateFlowers() {
        int RandomNum = Random.Range(0, 4);

        for (int i = 0; i < RandomNum; i++)
        {
            Vector3 randomLocation = RandomPointInBounds(myCollider.bounds);

            GameObject newFlower = Instantiate(flowerPrefab, randomLocation, transform.rotation);
            newFlower.transform.SetParent(transform);
        }
    }

    public void DeleteFlowers () {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static Vector3 RandomPointInBounds(Bounds bounds) {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
