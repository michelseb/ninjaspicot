using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeplacementFurtif : Deplacement {

    [SerializeField]
    private float propulseStrengthMax = 100;
    private float propulseStrengthSteps = 2f;
    bool loadingProcess;

    // Use this for initialization
    void Start () {
        SetMaxJumps(2);
        strength = 10;
        canAttach = false;
        //propulseStrengthMax = 100;
    }
	
	// Update is called once per frame
	void Update () {

        if (loadingProcess == true)
        {
            //GainEnergy();
            Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector2 forceToApply = (new Vector2(transform.position.x, transform.position.y) - clickToWorld) * strength;
            if (forceToApply.magnitude > propulseStrengthMax)
            {
                forceToApply = forceToApply.normalized * propulseStrengthMax;
            }
            t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, forceToApply.magnitude);
        }

        /*if (strength >= propulseStrengthMax)
        {
            Jump(Input.mousePosition, strength);
            loadingProcess = false;
            //InitEnergy();
        }*/

        if (Input.GetButtonDown("Fire1") && GetJumps() > 0 && loadingProcess == false)
        {
            loadingProcess = true;
        }
        else if (Input.GetButtonUp("Fire1") && GetJumps() > 0) //&& jumped == false)
        {
            Jump(Input.mousePosition, strength);
            jumped = true;
            loadingProcess = false;
            //InitEnergy();
        }

    }

    public override void Jump(Vector2 click, float strength)
    {
        StartCoroutine(t.FadeAway());
        r.velocity = new Vector2(0, 0);
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 forceToApply = (new Vector2(transform.position.x, transform.position.y) - clickToWorld) * strength;
        if (forceToApply.magnitude > propulseStrengthMax)
        {
            forceToApply = forceToApply.normalized * propulseStrengthMax;
        }
        r.AddForce(forceToApply, ForceMode2D.Impulse);
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
