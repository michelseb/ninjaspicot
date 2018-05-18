using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    ViewRange v;
    Turret t;
	// Use this for initialization
	void Start () {
        v = FindObjectOfType<ViewRange>();
        t = transform.parent.gameObject.GetComponent<Turret>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            v.s.material.color = new Color(1, 0, 0, .8f);
            if (t.autoShoot == false)
            {
                t.SendMessage("SelectMode", "aim");
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            if (t.autoShoot == false)
            {
                t.SendMessage("SelectMode", "search");
            }
        }
    }
}
