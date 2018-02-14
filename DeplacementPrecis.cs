using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPrecis : Deplacement {

	// Use this for initialization
	void Start () {
        SetMaxJumps(1);
        strength = 70;
        canAttach = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Fire1"))
        {
            t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, strength);
        }
        if (Input.GetButtonUp("Fire1") && GetJumps() > 0) //&& jumped == false)
        {
            Jump(Input.mousePosition, strength);
            jumped = true;
        }

    }
}
