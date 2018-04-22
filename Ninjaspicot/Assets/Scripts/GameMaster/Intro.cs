using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {

    GUIText g;
    Color c;
	// Use this for initialization
	void Start () {
        g = GetComponent<GUIText>();
        c = g.color;
        StartCoroutine(Introduction());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Introduction()
    {
        yield return new WaitForSeconds(2);
        while (c.a < 1)
        {
            
            c.a += .01f;
            g.color = c;
            yield return null;
        }
        yield return new WaitForSeconds(5);
        while (c.a > 0)
        {
            c.a -= .01f;
            g.color = c;
            yield return null;
        }

    }
}
