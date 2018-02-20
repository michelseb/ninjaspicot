using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    Vector2 propulseVector;
    bool jumpCapable;
    TimeManager time;
    Trigger tri;
    // Use this for initialization
    void Start () {
        SetMaxJumps(2);
        GainAllJumps();
        strength = 80;
        canAttach = true;
        time = FindObjectOfType<TimeManager>();
        tri = FindObjectOfType<Trigger>();
        //propulseStrengthMax = 100;
    }

    void Update()
    {

        if (Mathf.Abs(r.velocity.y) < 20)
        {
            if (jumpCapable == false)
            {
                GetComponent<SpriteRenderer>().color = Color.yellow;
                tri.ActivateParticles();
                jumpCapable = true;
                
            }
            SetJumps(1);
        }
        else
        {
            if (jumpCapable == true)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                tri.DeactivateParticles();
                jumpCapable = false;
                StartCoroutine(time.RestoreTime());
                SetJumps(0);
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            originClic = Input.mousePosition;
        }
        if (Input.GetButton("Fire1") && GetJumps() > 0)
        {
            propulseVector = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            if ((propulseVector - originClic).magnitude * strength > 2 && jumpCapable) {
                //if (Physics2D.Raycast(transform.position, propulseVector, 2) == false)
                //{
                    t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, strength);
                //}
            }
        }
        if (Input.GetButtonUp("Fire1") && GetJumps() > 0) //&& jumped == false)
        {
            Jump(Input.mousePosition, strength);
            jumped = true;
        }


    }

   

}
