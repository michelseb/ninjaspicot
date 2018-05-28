using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : Cloud {

    Vector2 originPos;
    public int xWay = 1, yWay = 1;
    public int moveX, moveY;
    bool reachedX, reachedY;
    public float speed;
    Rigidbody2D r;
    

    // Use this for initialization
    void Start () {
        r = GetComponent<Rigidbody2D>();

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

    


}
