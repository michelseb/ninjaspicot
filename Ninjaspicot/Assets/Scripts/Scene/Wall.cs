using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public Direction myDir;
    private bool isGrabbed = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
