using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewRange : MonoBehaviour {

    public SpriteRenderer s;
    Turret t;

	// Use this for initialization
	void Start () {
        s = GetComponent<SpriteRenderer>();
        t = transform.parent.gameObject.GetComponent<Turret>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            s.material.color = new Color(.6f, 0, 1, .8f);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            s.material.color = new Color(0, 1, 0, .8f);
            t.facteur = 1;
        }
        
    }
}
