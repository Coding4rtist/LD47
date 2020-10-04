using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      transform.position = mousePos;
   }
}
