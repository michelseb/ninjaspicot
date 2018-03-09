using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    Vector2 propulseVector;
    bool jumpCapable, readyToJump;

    Trigger tri;
    Ninja n;
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
            if ((propulseVector - originClic).magnitude * strength > 2 && jumpCapable) {
                //if (Physics2D.Raycast(transform.position, propulseVector, 2) == false)
                //{
                t.Reduce();
                t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                //}
            }
        }
        if (Input.GetButtonUp("Fire1") && GetJumps() > 0 && readyToJump) //&& jumped == false)
        {
            Jump(Input.mousePosition, strength);
            jumped = true;
            readyToJump = false;
            
        }


    }

   

}
