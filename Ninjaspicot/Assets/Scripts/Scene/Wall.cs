using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]
public class Wall : MonoBehaviour {

    Ninja n;
    Deplacement d;
    public GameObject contact;
    public ContactPoint2D c;

    // Use this for initialization
    private void Awake()
    {
        n = FindObjectOfType<Ninja>();
        d = FindObjectOfType<Deplacement>();
    }
    private void Start()
    {
        contact = new GameObject();
        contact.transform.position = transform.position;
        contact.transform.parent = transform;
        
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
            c = collision.contacts[collision.contacts.Length - 1];
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
        if (collision.gameObject.tag == "ninja")
        {
            c = collision.contacts[collision.contacts.Length - 1];
            n.lastColliders.Enqueue(gameObject);
            PositionContact(c.point);
        }
        
    }

    public void PositionContact(Vector3 pos)
    {
        contact.transform.position = pos;
    }
    /* void OnDrawGizmos()
     {
         Gizmos.color = Color.yellow;
         Gizmos.DrawSphere(contact.transform.position, 1f);
     }

         void OnDrawGizmos()
         {
             Gizmos.color = Color.yellow;
             foreach (ContactPoint2D c in contacts)
             {*
         Gizmos.DrawSphere(c.point, .5f);
             //}
         }*/
}
