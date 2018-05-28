using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBody : MonoBehaviour {

    Ninja n;
    Deplacement d;
    public float pullForce;
    Rigidbody2D r;
    Wall w;
    

    // Use this for initialization
    void Start () {
        n = FindObjectOfType<Ninja>();
        d = FindObjectOfType<Deplacement>();
        w = GetComponent<Wall>();
        r = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {

        if (n.currCollider == gameObject && d.isSticking)// && n.gameObject.GetComponent<HingeJoint2D>() == null)
        {
            
            Debug.DrawLine(transform.TransformPoint(d.hinge.anchor), transform.TransformPoint(d.hinge.connectedAnchor), Color.cyan);
            Vector2 forceDirection = w.contact.transform.position - n.transform.position;
            n.r.AddForce(forceDirection * 1000, ForceMode2D.Force);
            //d.hinge.connectedAnchor = transform.InverseTransformPoint(n.contact.point);

            //d.hinge.anchor = transform.InverseTransformPoint(w.contact.transform.position);
        }
        else
        {
            
        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            d.isAttached = false;
            d.isWalking = false;
            
            if (collision.gameObject.GetComponent<HingeJoint2D>() != null)
            {
                Destroy(collision.gameObject.GetComponent<HingeJoint2D>());
            }
        }
    }

}
