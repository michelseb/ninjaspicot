using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifePivot : MonoBehaviour {

    public float speed;
    float initSpeed;
    Knife k;
    Rigidbody2D r;
	// Use this for initialization
	void Start () {
        k = transform.GetComponentInChildren<Knife>();
        r = GetComponent<Rigidbody2D>();
        initSpeed = speed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        r.MoveRotation(r.rotation + speed * Time.deltaTime);
        //transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
        if (speed > -200)
        {
            speed += initSpeed * Time.deltaTime;
        }
		if (k.touched)
        {
            speed = -initSpeed;
        }
	}
}
