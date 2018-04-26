using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    
    Vector2 propulseVector;
    bool readyToJump;
    Collider2D col;
    Trigger tri;
    Coroutine walkOnWalls;
    Wall wall = null;
    bool isWalking;
    public int rapidite;
    Touch to;

    void Start () {
        n = GetComponent<Ninja>();
        SetMaxJumps(2);
        GainAllJumps();
        strength = 80;
        canAttach = true;
        tri = FindObjectOfType<Trigger>();
        to = FindObjectOfType<Touch>();
    }

    void Update()
    { 
        
        if (Input.GetButtonDown("Fire1"))
        {
            
            originClic = Input.mousePosition;
            StartCoroutine(to.CreatePoints(originClic));
            if (originClic.x < Screen.width / 2)
            {
                ninjaDir = Dir.Left;
            }
            else
            {
                ninjaDir = Dir.Right;
            }
            readyToJump = true;
            t.Reset();

        }

        if (readyToJump && GetJumps() > 0)
        {
            propulseVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (isAttached && hinge != null)
            {
                
                if ((propulseVector - originClic).magnitude < 40)
                {
                    if (isWalking == false && to.canGo == true)
                    {
                        walkOnWalls = StartCoroutine(WalkOnWalls());
                    }
                    
                    StartCoroutine(t.FadeAway());
                    time.NormalTime();
                }
                if ((propulseVector - originClic).magnitude> 40)
                {
                    //t.Reduce();
                    t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                }

            }
            else
            {
                if ((propulseVector - originClic).magnitude > 40)
                {
                    //t.Reduce();
                    t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                }

                if ((propulseVector - originClic).magnitude < 40)
                {
                    StartCoroutine(t.FadeAway());
                    time.NormalTime();
                }
            }
        }
        if (Input.GetButtonUp("Fire1") && GetJumps() > 0 && readyToJump)
        {
            to.Erase();
            RaycastHit2D hit = Physics2D.Linecast(transform.position, t.line.GetPosition(2), LayerMask.GetMask("Default"));
            Debug.DrawLine(transform.position, t.line.GetPosition(2), Color.green);
            if (hit && hit.collider.tag != "ninja")
            {
                StartCoroutine(t.FadeAway());
                StartCoroutine(time.RestoreTime());
            }
            else
            {
                if ((propulseVector - originClic).magnitude > 40)
                {
                    if (isWalking == true)
                    {
                        StopCoroutine(walkOnWalls);
                        isWalking = false;
                    }

                    Jump(Input.mousePosition, strength);
                }
            }
            jumped = true;
            readyToJump = false;
        }


    }


    public IEnumerator WalkOnWalls()
    {
        
        isWalking = true;
         
        if (col != null)
        {
            wall = col.gameObject.GetComponent<Wall>();
            Debug.Log(wall);
        }
        
        motor = hinge.motor;
        while (Input.GetButton("Fire1"))
        {
            col = n.contact.collider;

            if (ninjaDir == Dir.Right)
            {
                motor.motorSpeed = rapidite;
            }
            else if (ninjaDir == Dir.Left)
            {
                motor.motorSpeed = -rapidite;
            }

            hinge.motor = motor; 
            hinge.anchor = transform.InverseTransformPoint(n.contact.point);

            yield return null;
        }
        motor.motorSpeed = 0;
        hinge.motor = motor;
        hinge.anchor = transform.InverseTransformPoint(n.contact.point);
        isWalking = false;

    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(n.contact.point, .5f);
    }*/


}
