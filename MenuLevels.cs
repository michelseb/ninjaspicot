using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLevels : MonoBehaviour {

    ScrollRect s;
    ScenesManager sc;
	// Use this for initialization
	void Start () {
        sc = FindObjectOfType<ScenesManager>();
        s = GetComponent<ScrollRect>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void GenerateMenuLevels()
    {
        
        for (int a = 0; a < sc.GetNumberOfLevels(); a++)
        {
            
        }
    }
}
