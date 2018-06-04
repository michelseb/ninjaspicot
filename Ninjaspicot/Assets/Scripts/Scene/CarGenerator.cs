using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGenerator : MonoBehaviour {

    public float coolDown;
    private float timer;
    private bool canGenerate;
    public GameObject car;

	// Use this for initialization
	void Start () {
        timer = coolDown;
        canGenerate = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer > 0)
        {
            timer -= Time.deltaTime;
            canGenerate = true;
        }
        else
        {
            if (canGenerate)
            {
                timer = coolDown;
                GenerateCar();
                canGenerate = false;
            }
        }
	}

    private void GenerateCar()
    {
        Instantiate(car, transform.position, Quaternion.identity);
    }
}
