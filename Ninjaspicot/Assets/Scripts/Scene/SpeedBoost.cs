using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoost : MonoBehaviour {

    Deplacement d;

    private void Start()
    {
        d = FindObjectOfType<Deplacement>();
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            StartCoroutine(d.SpeedBoost(3));
        }
    }
}
