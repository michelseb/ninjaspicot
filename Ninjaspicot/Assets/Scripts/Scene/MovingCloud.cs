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

    }

    private void FixedUpdate()
    {
        
        if (n.currCollider == gameObject && d.isSticking) //&& n.gameObject.GetComponent<HingeJoint2D>() == null)
        {
            Vector3 forceDirection = transform.position - n.transform.position;
            n.r.AddForce(forceDirection.normalized * pullForce * Time.fixedDeltaTime, ForceMode2D.Force);
        }
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
