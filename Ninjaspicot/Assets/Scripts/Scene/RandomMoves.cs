using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoves : MonoBehaviour {

    Rigidbody2D r;
	// Use this for initialization
	void Start () {
        r = GetComponent<Rigidbody2D>();
        StartCoroutine(Move());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
    IEnumerator Move()
    {
        r.AddForce(new Vector2(Random.Range(-10000, 10000), Random.Range(-10000, 10000)), ForceMode2D.Impulse);
        yield return new WaitForSeconds(2);
        StartCoroutine(Move());
    }


}
