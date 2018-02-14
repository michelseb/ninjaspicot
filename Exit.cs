using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    private void Update()
    {
        transform.Rotate(0, 0, 120);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            ScenesManager.NextScene();
            Destroy(gameObject);
        }
        
    }
}
