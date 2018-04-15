using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    Renderer r;
	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

	}


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            Ninja n = GameObject.Find("Ninjaspicot").GetComponent<Ninja>();
            n.Die(transform);
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "GrabableWall")
        {
            Destroy(gameObject);
        }

    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void Fragment()
    {

    }
}
