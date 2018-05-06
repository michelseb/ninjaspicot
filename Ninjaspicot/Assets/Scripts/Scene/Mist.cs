using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mist : MonoBehaviour {
    ParticleSystem p;
	// Use this for initialization
	void Start () {
        p = GetComponent<ParticleSystem>();
        p.GetComponent<Renderer>().sortingLayerName = "Foreground";
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
