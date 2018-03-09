using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perimeter : MonoBehaviour {


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            Ninja n = GameObject.Find("Ninjaspicot").GetComponent<Ninja>();
            n.Die(transform);
        }
    }
}
