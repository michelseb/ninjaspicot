using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {

    CameraBehaviour c;
    public bool inZoom;
    public int zoomAmount;
	// Use this for initialization
	void Start () {
        c = FindObjectOfType<CameraBehaviour>();
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (inZoom)
        {
            StartCoroutine(c.zoomIn(zoomAmount));
        }
        else
        {
            StartCoroutine(c.zoomOut(zoomAmount));
        }
        
    }
}
