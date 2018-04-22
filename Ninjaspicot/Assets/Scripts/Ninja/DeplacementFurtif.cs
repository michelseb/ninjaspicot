using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    
    Vector2 propulseVector;
    bool jumpCapable, readyToJump;
    Collider2D col;
    Trigger tri;
    Coroutine walkOnWalls;
    Wall wall = null;
    bool isWalking;
    // Use this for initialization
    void Start () {
        n = GetComponent<Ninja>();
        SetMaxJumps(2);
        GainAllJumps();
        strength = 80;
        canAttach = true;
        tri = FindObjectOfType<Trigger>();
        //propulseStrengthMax = 100;
    }

    void Update()
    {

        /*if (isAttached)
        {
            if (hinge == null)
            {
                hinge = gameObject.GetComponent<HingeJoint2D>();
            }
            if (n.contact.point.x < transform.position.x)
            {
                hinge.anchor = transform.InverseTransformPoint(n.contact.point);
                r.AddForce(new Vector2(1, 1) * strength * 5, ForceMode2D.Force);
            }
            else
            {
                hinge.anchor = transform.InverseTransformPoint(n.contact.point);
                r.AddForce(new Vector2(1, 1) * strength * 5, ForceMode2D.Force);
            }

            
        }*/
        //if (Mathf.Abs(r.velocity.y) < 20)
        //{
        if (jumpCapable == false)
        {
            //GetComponent<SpriteRenderer>().color = Color.yellow;
            jumpCapable = true;
                
        }
        //}
        /*else
        {
            if (jumpCapable == true)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                tri.DeactivateParticles();
                jumpCapable = false;
                //StartCoroutine(time.RestoreTime());
                SetJumps(0);
            }
        }*/
        if (Input.GetButtonDown("Fire1") && jumpCapable)
        {
            
            originClic = Input.mousePosition;
            readyToJump = true;
            t.Reset();

        }/*else if(Input.GetButtonDown("Fire1") && jumpCapable == false)
        {
            n.Die(null);
        }*/
        if (readyToJump && GetJumps() > 0)
        {
            propulseVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if (isAttached && hinge != null)
            {
                col = n.contact.collider;
                if (col != null)
                {
                    wall = col.gameObject.GetComponent<Wall>();
                    if (wall != null)
                    {
                        if (wall.myDir == Wall.Direction.Left)
                        {
                            ninjaDir = Dir.Left;
                        }
                        else
                        {
                            ninjaDir = Dir.Right;
                        }
                    }
                    
                }
                if ((propulseVector - originClic).magnitude < 40 && isWalking == false)
                {
                    walkOnWalls = StartCoroutine(WalkOnWalls());
                }
                if ((propulseVector - originClic).magnitude> 40 && jumpCapable)
                {
                    t.Reduce();
                    t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                }

            }
            else
            {
                if ((propulseVector - originClic).magnitude > 40 && jumpCapable)
                {
                    t.Reduce();
                    t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                }
            }
        }
        if (Input.GetButtonUp("Fire1") && GetJumps() > 0 && readyToJump) //&& jumped == false)
        {
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

            /*RaycastHit2D hitleft = Physics2D.Linecast(transform.position, transform.position+new Vector3(-15,0,0), LayerMask.GetMask("Default"));
            RaycastHit2D hitright = Physics2D.Linecast(transform.position, transform.position + new Vector3(15, 0, 0), LayerMask.GetMask("Default"));
            RaycastHit2D hitup = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, 2, 0), LayerMask.GetMask("Default"));
            RaycastHit2D hitdown = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, -2, 0), LayerMask.GetMask("Default"));

            Debug.DrawLine(transform.position, transform.position + new Vector3(15, 0, 0), Color.green);
            Debug.DrawLine(transform.position, transform.position + new Vector3(-15, 0, 0), Color.red);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, 2, 0), Color.blue);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, -2, 0), Color.yellow);*/

            col = n.contact.collider;
            

            if (wall != null)
            {
                if (wall.myDir == Wall.Direction.Right)
                {
                    Debug.Log("right");
                    motor.motorSpeed = 500;
                    //r.AddForce(new Vector2(.3f, 1).normalized * strength * 10, ForceMode2D.Force);
                }
                else if (wall.myDir == Wall.Direction.Left)
                {
                    Debug.Log("left");
                    motor.motorSpeed = -500;
                    //r.AddForce(new Vector2(-.3f, 1).normalized * strength * 10, ForceMode2D.Force);
                }
            }
            /*else if (hitdown)
            {
                r.AddForce(new Vector2(-.3f, 1).normalized * strength * 10, ForceMode2D.Force);
            }
            else if (hitup)
            {
                r.AddForce(new Vector2(1, .1f).normalized * strength * 10, ForceMode2D.Force);
            }
            */
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
