using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    Vector2 propulseVector;
    bool jumpCapable, readyToJump;
    HingeJoint2D hinge;
    Trigger tri;
    Ninja n;
    Coroutine walkOnWalls;
    bool isWalking;
    // Use this for initialization
    void Start () {
        n = GetComponent<Ninja>();
        SetMaxJumps(10);
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
            GetComponent<SpriteRenderer>().color = Color.yellow;
            tri.ActivateParticles();
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
            if (isAttached)
            {
                if ((propulseVector - originClic).magnitude < 100 && isWalking == false)
                {
                    walkOnWalls = StartCoroutine(WalkOnWalls());
                }
                if (t.verts > 2)
                {
                    if ((propulseVector - originClic).magnitude > 100 && isWalking == true)
                    {
                        StopCoroutine(walkOnWalls);
                        isWalking = false;
                        Debug.Log("Stop");
                        
                    }
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, t.line.GetPosition(2), LayerMask.GetMask("Default"));
                    Debug.DrawLine(transform.position, t.line.GetPosition(2), Color.green);
                    if (hit == false)
                    {
                        if ((propulseVector - originClic).magnitude > 100 && jumpCapable)
                        {
                            t.Reduce();
                            t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                        }
                    }
                    else
                    {
                        
                        StartCoroutine(t.FadeAway());
                        readyToJump = false;
                        
                    }
                }
                else
                {
                    if ((propulseVector - originClic).magnitude> 100 && jumpCapable)
                    {
                        t.Reduce();
                        t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                    }
                }
            }
            else
            {
                if ((propulseVector - originClic).magnitude > 100 && jumpCapable)
                {
                    t.Reduce();
                    t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                }
            }
        }
        if (Input.GetButtonUp("Fire1") && GetJumps() > 0 && readyToJump && (propulseVector - originClic).magnitude > 100) //&& jumped == false)
        {
            if (isWalking == true)
            {
                StopCoroutine(walkOnWalls);
                isWalking = false;
            }
            Jump(Input.mousePosition, strength);
            jumped = true;
            readyToJump = false;
            
        }


    }


    public IEnumerator WalkOnWalls()
    {
        
        isWalking = true;
        hinge = gameObject.GetComponent<HingeJoint2D>();
        while (Input.GetButton("Fire1"))
        {
            RaycastHit2D hitleft = Physics2D.Linecast(transform.position, transform.position+new Vector3(-10,0,0), LayerMask.GetMask("Default"));
            RaycastHit2D hitright = Physics2D.Linecast(transform.position, transform.position + new Vector3(10, 0, 0), LayerMask.GetMask("Default"));
            RaycastHit2D hitup = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, 2, 0), LayerMask.GetMask("Default"));
            RaycastHit2D hitdown = Physics2D.Linecast(transform.position, transform.position + new Vector3(0, -2, 0), LayerMask.GetMask("Default"));

            Debug.DrawLine(transform.position, transform.position + new Vector3(10, 0, 0), Color.green);
            Debug.DrawLine(transform.position, transform.position + new Vector3(-10, 0, 0), Color.red);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, 2, 0), Color.blue);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, -2, 0), Color.yellow);


            if (hitleft && hitup)
            {
                
                r.AddForce(new Vector2(1, 1).normalized * strength * 10, ForceMode2D.Force);
            }
            else if (hitleft && hitdown)
            {
                r.AddForce(new Vector2(-1, 1).normalized * strength * 10, ForceMode2D.Force);
            }
            else if (hitright && hitup)
            {
                r.AddForce(new Vector2(-1, 1).normalized * strength * 10, ForceMode2D.Force);
            }
            else if (hitright && hitdown)
            {
                r.AddForce(new Vector2(1, 1).normalized * strength * 10, ForceMode2D.Force);
            }
            else if (hitright)
            {
                Debug.Log("right");
                r.AddForce(new Vector2(.3f, 1).normalized * strength * 10, ForceMode2D.Force);
            }
            else if (hitleft)
            {
                Debug.Log("left");
                r.AddForce(new Vector2(-.3f, 1).normalized * strength * 10, ForceMode2D.Force);
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
            hinge.anchor = transform.InverseTransformPoint(n.contact.point);
           
            
            
            yield return null;
        }
        isWalking = false;

    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(n.contact.point, .5f);
    }*/


}
