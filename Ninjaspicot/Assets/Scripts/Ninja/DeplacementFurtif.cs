using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    
    Vector2 propulseVector;
    Trigger tri;
    Coroutine walkOnWalls;
    public int maxJumps;
    
    Touch to;
    

    void Start () {
        n = GetComponent<Ninja>();
        SetMaxJumps(maxJumps);
        GainAllJumps();
        strength = 80;
        tri = FindObjectOfType<Trigger>();
        to = FindObjectOfType<Touch>();
        OriginalRapidite = rapidite;
        StartCoroutine(cam.zoomIntro());
    }

    void Update()
    {


        if (started)
        {
            //Debug.Log(n.contact.point.x + " " + n.contact.point.y);
            if (Input.GetButtonDown("Fire1") && GetJumps() > 0)
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
                Vector3 originClicToWorld = c.ScreenToWorldPoint(originClic);
                Vector3 propulseVectorToWorld = c.ScreenToWorldPoint(propulseVector);
                if (isAttached && hinge != null)
                {

                    if ((propulseVectorToWorld - originClicToWorld).magnitude < 5)
                    {
                        if (isWalking == false)
                        {
                            walkOnWalls = StartCoroutine(WalkOnWalls());
                        }

                        StartCoroutine(t.FadeAway());
                        time.NormalTime();
                    }
                    if ((propulseVectorToWorld - originClicToWorld).magnitude > 5)
                    {
                        if (isWalking == true)
                        {
                            isWalking = false;
                        }
                        //t.Reduce();
                        t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                    }

                }
                else
                {
                    if ((propulseVectorToWorld - originClicToWorld).magnitude > 5)
                    {
                        if (isWalking == true)
                        {
                            isWalking = false;
                        }
                        //t.Reduce();
                        t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                    }

                    if ((propulseVectorToWorld - originClicToWorld).magnitude < 5)
                    {
                        StartCoroutine(t.FadeAway());
                        time.NormalTime();
                    }
                }
            }
            if (Input.GetButtonUp("Fire1") && GetJumps() > 0 && readyToJump && t.line.positionCount > 1)
            {
                to.Erase();
                Vector3 originClicToWorld = c.ScreenToWorldPoint(originClic);
                Vector3 propulseVectorToWorld = c.ScreenToWorldPoint(propulseVector);
                RaycastHit2D hit = Physics2D.Linecast(transform.position, t.line.GetPosition(2), LayerMask.GetMask("Default"));
                Debug.DrawLine(transform.position, t.line.GetPosition(2), Color.green);
                if (hit && hit.collider.tag != "ninja")
                {
                    StartCoroutine(t.FadeAway());
                    StartCoroutine(time.RestoreTime());
                }
                else
                {
                    if ((propulseVectorToWorld - originClicToWorld).magnitude > 5)
                    {
                        if (isWalking == true)
                        {
                            isWalking = false;
                        }

                        Jump(Input.mousePosition, strength);
                    }
                }
                readyToJump = false;
            }
        }

    }

    private void FixedUpdate()
    {
        if (isAttached && readyToJump == false)
        {
            r.velocity = new Vector2(0, 0);
        }

        if (hinge != null)
        {
            hinge.anchor = transform.InverseTransformPoint(n.contact.point);
            hinge.connectedAnchor = transform.InverseTransformPoint(n.contact.point);
        }
    }


    public IEnumerator WalkOnWalls()
    {
        
        isWalking = true;
        if (hinge != null)
        {
            motor = hinge.motor;
            while (Input.GetButton("Fire1") && isWalking == true)
            {
                if (hinge != null)
                {
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
                    hinge.connectedAnchor = transform.InverseTransformPoint(n.contact.point);
                }
                yield return null;
            }
            if (hinge != null)
            {
                motor.motorSpeed = 0;
                hinge.motor = motor;
                hinge.anchor = transform.InverseTransformPoint(n.contact.point);
                hinge.connectedAnchor = transform.InverseTransformPoint(n.contact.point);
                isWalking = false;
            }
        }

    }
    /*
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0,0,.6f);
        if (hinge != null)
        {
            Gizmos.DrawSphere(transform.TransformPoint(hinge.anchor), 1f);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(n.contact.point, .5f);
        foreach (ContactPoint2D c in n.contacts)
        {
            Gizmos.DrawSphere(c.point, .5f);
        }
    }
*/

}
