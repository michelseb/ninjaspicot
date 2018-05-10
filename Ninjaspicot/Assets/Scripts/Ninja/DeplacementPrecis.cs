using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementPrecis : Deplacement {

    [SerializeField]
    private float propulseStrengthMax = 100;
    private float propulseStrengthSteps = 2f;
    bool loadingProcess;

    // Use this for initialization
    void Start () {
        SetMaxJumps(1);
        strength = 70;
    }

    // Update is called once per frame
    // Update is called once per frame
    void Update()
    {

        if (loadingProcess == true)
        {
            //GainEnergy();
            Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector2 originClickToWorld = c.ScreenToWorldPoint(new Vector3(originClic.x, originClic.y, 0));
            Vector2 forceToApply = (originClickToWorld - clickToWorld) * strength;
            if (forceToApply.magnitude > propulseStrengthMax)
            {
                forceToApply = forceToApply.normalized * propulseStrengthMax;
            }
            if (forceToApply.magnitude > 2)
            {
                t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, originClic, forceToApply.magnitude);
            }
        }

        /*if (strength >= propulseStrengthMax)
        {
            Jump(Input.mousePosition, strength);
            loadingProcess = false;
            //InitEnergy();
        }*/

        if (Input.GetButtonDown("Fire1") && GetJumps() > 0 && loadingProcess == false)
        {
            originClic = Input.mousePosition;
            loadingProcess = true;
        }
        else if (Input.GetButtonUp("Fire1") && GetJumps() > 0) //&& jumped == false)
        {
            Jump(Input.mousePosition, strength);
            loadingProcess = false;
            //InitEnergy();
        }

    }

    public override void Jump(Vector2 click, float strength)
    {
        StartCoroutine(t.FadeAway());
        Detach();
        r.velocity = new Vector2(0, 0);
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 originClickToWorld = c.ScreenToWorldPoint(new Vector3(originClic.x, originClic.y, 0));
        Vector2 forceToApply = (originClickToWorld - clickToWorld) * strength;
        if (forceToApply.magnitude > propulseStrengthMax)
        {
            forceToApply = forceToApply.normalized * propulseStrengthMax;
        }
        if (forceToApply.magnitude < 2)
        {
            if (click.x < Screen.width / 3)
            {
                r.AddForce((Vector2.up - Vector2.left) * strength, ForceMode2D.Impulse);
            }
            else if (click.x > Screen.width / 3 && click.x < 2 * Screen.width / 3)
            {
                r.AddForce(Vector2.up * strength, ForceMode2D.Impulse);
            }
            else
            {
                r.AddForce((Vector2.up + Vector2.left) * strength, ForceMode2D.Impulse);
            }
        }
        else
        {
            r.AddForce(forceToApply, ForceMode2D.Impulse);
        }
        
    }

    /*private void GainEnergy()
    {
        if (strength < propulseStrengthMax)
        {
            strength += propulseStrengthSteps;
        }
    }

    private void InitEnergy()
    {
        strength = 0;
    }*/
}
