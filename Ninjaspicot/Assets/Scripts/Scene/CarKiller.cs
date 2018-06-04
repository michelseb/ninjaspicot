using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarKiller : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "auto")
        {
            Destroy(collision.gameObject.transform.parent.gameObject);
        }
    }
}
