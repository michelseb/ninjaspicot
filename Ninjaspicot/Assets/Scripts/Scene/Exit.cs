<<<<<<< HEAD:Ninjaspicot/Assets/Scripts/Scene/Exit.cs
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            ScenesManager.NextScene();
        }
        
    }
}
=======
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    private void Update()
    {
        transform.Rotate(0, 0, 120);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            ScenesManager.NextScene();
            Destroy(gameObject);
        }
        
    }
}
>>>>>>> 0ef3c2388dae75248a06e0197436361176b4abb9:Exit.cs
