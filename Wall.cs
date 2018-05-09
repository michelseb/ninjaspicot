using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class Wall : MonoBehaviour {

    private bool isGrabbed = false;
    Ninja n;
    Deplacement d;

    // Use this for initialization
    private void Awake()
    {
        n = FindObjectOfType<Ninja>();
        d = FindObjectOfType<Deplacement>();
    }
	
	// Update is called once per frame
	void Update () {
        if (d == null)
        {
            d = FindObjectOfType<Deplacement>();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            if (n.currCollider == null)
            {
                n.currCollider = gameObject;
            }

            if (n.CheckRecentCollider(gameObject))
            {
                n.contact = collision.contacts[collision.contacts.Length - 1];
                n.currCollider = gameObject;
            }

            if (d != null)
            {
                d.ReactToGround(gameObject);

            }
        }
    }


    public void OnCollisionStay2D(Collision2D collision)
    {
        n.lastColliders.Enqueue(gameObject);
    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (ContactPoint2D c in contacts)
        {*
    Gizmos.DrawSphere(c.point, .5f);
        //}
    }*/
}
