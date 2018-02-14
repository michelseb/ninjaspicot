<<<<<<< HEAD:Ninjaspicot/Assets/Scripts/Scene/Turrets/Bullet.cs
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            Ninja n = GameObject.Find("Ninjaspicot").GetComponent<Ninja>();
            n.Die();
        }
        Destroy(gameObject);
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            Ninja n = GameObject.Find("Ninjaspicot").GetComponent<Ninja>();
            n.Die();
        }
        Destroy(gameObject);
    }

    private void Fragment()
    {

    }
}
>>>>>>> 0ef3c2388dae75248a06e0197436361176b4abb9:Bullet.cs
