using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public int speed;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            Ninja n = GameObject.Find("Ninjaspicot").GetComponent<Ninja>();
            n.Die(null);
        }
    }
}
