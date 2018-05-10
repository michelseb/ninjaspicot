using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : Cloud {

    Vector2 originPos;
    public 
    int xWay = 1, yWay = 1;
    public int moveX, moveY;
    bool reachedX, reachedY;
    public float speed;
    Rigidbody2D r;
    Animation a;
    Ninja n;
    Deplacement d;
    bool ninjaAttached;
    public float pullForce;

    // Use this for initialization
    void Start () {
        r = GetComponent<Rigidbody2D>();
        n = FindObjectOfType<Ninja>();
        d = FindObjectOfType<Deplacement>();
        //a = GetComponent<Animation>();
        originPos = r.position;
        if (moveX == 0)
            xWay = 0;
        if (moveY == 0)
            yWay = 0;
        //a["HorizontalMove"].speed = .1f;
    }
	
	// Update is called once per frame
	void Update () {
        r.MovePosition(r.position + new Vector2(xWay, yWay)*speed*Time.deltaTime);//AddForce(new Vector2(xWay * speed * Time.deltaTime, yWay * speed * Time.deltaTime), ForceMode2D.Force);
        if (r.position.x - originPos.x > moveX && reachedX == false)
        {
            xWay = -1;
            reachedX = true;
        }else if (r.position.x - originPos.x < -moveX && reachedX == true)
        {
            xWay = 1;
            reachedX = false;
        }
        if (r.position.y - originPos.y > moveY && reachedY == false)
        {
            yWay = -1;
            reachedY = true;
        }
        else if (r.position.y - originPos.y < -moveY && reachedY == true)
        {
            yWay = 1;
            reachedY = false;
        }

        if (d.isAttached == false && ninjaAttached == true)
        {
            ninjaAttached = false;
        }
    }

    private void FixedUpdate()
    {
        
        if (n.currCollider == gameObject && d.isSticking) //&& n.gameObject.GetComponent<HingeJoint2D>() == null)
        {
            Debug.Log("Gravity");
            Vector3 forceDirection = transform.position - n.transform.position;

            // apply force on target towards me
            n.r.AddForce(forceDirection.normalized * pullForce * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ninja" && ninjaAttached == false)
        {
            ninjaAttached = true;
        }
        /*if (collision.gameObject.GetComponent<HingeJoint2D>() == null)
        {
            HingeJoint2D hinge = collision.gameObject.AddComponent<HingeJoint2D>();
            hinge.useLimits = false;
            hinge.anchor = transform.InverseTransformPoint(n.contact.point);
            hinge.connectedAnchor = transform.InverseTransformPoint(n.contact.point);
            hinge.enableCollision = true;
            hinge.connectedBody = r;

            hinge.useMotor = true;
            JointMotor2D motor = hinge.motor;
            hinge.motor = motor;
            d.hinge = hinge;
        }*/
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        d.isAttached = false;
        d.isWalking = false;
        if (collision.gameObject.GetComponent<HingeJoint2D>() != null)
        {
            Destroy(collision.gameObject.GetComponent<HingeJoint2D>());
        }
    }

}
