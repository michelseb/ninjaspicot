using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    Ninja n;
	// Use this for initialization
	void Start () {
        n = FindObjectOfType<Ninja>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * 10 * Time.unscaledDeltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            n.Die(null);
        }
    }
}
