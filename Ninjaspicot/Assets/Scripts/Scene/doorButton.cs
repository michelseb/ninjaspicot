using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorButton : MonoBehaviour {

    bool actif;
    Door d;
    Material m;
	// Use this for initialization
	void Start () {
        d = transform.GetComponentInParent<Door>();
        m = GetComponent<SpriteRenderer>().material;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            if (actif)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }


    private void Open()
    {
        actif = true;
        m.color = Color.green;
        d.Activate();
    }
    private void Close()
    {
        actif = false;
        m.color = Color.red;
        //d.Activate();
    }
}
