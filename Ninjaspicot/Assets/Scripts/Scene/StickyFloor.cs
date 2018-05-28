using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFloor : MonoBehaviour {
    Deplacement d;

    private void Start()
    {
        d = FindObjectOfType<Deplacement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            if (d.GetJumps() > 0)
            {
                d.LoseJump();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            d.GainAllJumps();
        }
    }
}
