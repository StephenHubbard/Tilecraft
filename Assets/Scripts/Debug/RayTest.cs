using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        if (hit)
        {
            Debug.Log (hit.collider.gameObject.name);
        }
    }
}
