using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limit : MonoBehaviour {

    Ninja n;

    private void Awake()
    {
        n = FindObjectOfType<Ninja>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "ninja")
        {
            n.Die(null);
        }
        
    }
}
