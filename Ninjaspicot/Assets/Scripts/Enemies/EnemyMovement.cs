using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    Rigidbody2D r;
    HingeJoint2D hinge;
    bool isAttached, isWalking;
    JointMotor2D motor;
    ContactPoint2D contact;
    GameObject currCollider;
    public enum Dir
    {
        Left,
        Right
    }
    public Dir enemyDir;
    public float rapidite;

    // Use this for initialization
    void Start () {
        r = this.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCollisionStay2D(Collision2D collision)
    {
        ReactToGround(collision.gameObject);
        contact = collision.contacts[0];
    }

    public void Attach(Rigidbody2D ri, Wall w)
    {

        r.velocity = new Vector2(0, 0);
        hinge = gameObject.AddComponent<HingeJoint2D>();
        hinge.useLimits = false;
        hinge.anchor = transform.InverseTransformPoint(contact.point);
        w.PositionContact(w.c.point);
        hinge.connectedAnchor = transform.InverseTransformPoint(contact.point);
        hinge.enableCollision = true;
        hinge.connectedBody = ri;
        hinge.useMotor = true;
        JointMotor2D motor = hinge.motor;
        hinge.motor = motor;
        Debug.Log("Attached");

        isAttached = true;

    }


    public void WalkOnWalls()
    {

        isWalking = true;
        if (hinge != null)
        {
            motor = hinge.motor;

            if (enemyDir == Dir.Right)
            {
                motor.motorSpeed = rapidite;
            }
            else if (enemyDir == Dir.Left)
            {
                motor.motorSpeed = -rapidite;
            }
            hinge.motor = motor;
            hinge.anchor = transform.InverseTransformPoint(contact.point);
            hinge.connectedAnchor = transform.InverseTransformPoint(contact.point);
        }


    }

    public void ReactToGround(GameObject g)
    {
        //GainAllJumps();
        if (g.tag != "Wall")
        {
            if (isAttached == false)
            {
                //Detach();
                Attach(g.GetComponent<Rigidbody2D>(), g.GetComponent<Wall>());
            }
            else
            {
                WalkOnWalls();
            }
        }
    }
}
