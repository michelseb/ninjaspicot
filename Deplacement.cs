using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deplacement : MonoBehaviour {

    private Trajectoire t;
    private Rigidbody2D r;
    private Camera c;
    public float strength;
    private bool canJump, jumped, isImpulsing, isAttached;
    public float propulseStrength;
    Animator anim;
    [SerializeField]
    private float propulseStrengthMax;
    private float propulseStrengthSteps = 5f;
    bool loadingProcess;
    private enum Ground { grabable, bumpy };
    Ground ground;
    

    private void Awake()
    {
        r = this.GetComponent<Rigidbody2D>();
        c = GameObject.Find("Camera").GetComponent<Camera>();
        t = c.gameObject.GetComponent<Trajectoire>();
        anim = GetComponent<Animator>();
        
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButton("Fire1"))
        {
            t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, strength);
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Detach();
            Jump(Input.mousePosition);
            StartCoroutine(t.FadeAway());
        }
       
        
        if (Input.GetButtonDown("Fire1") && jumped && isImpulsing == false && loadingProcess == false)
        {
            loadingProcess = true;
            //Jump(Input.mousePosition);
        }
        else if (Input.GetButtonUp("Fire1") && jumped && isImpulsing == false && loadingProcess == true)
        {
            //Propulse(Input.mousePosition);
            Jump(Input.mousePosition);
            loadingProcess = false;
            isImpulsing = true;
            InitEnergy();
        }

        if (loadingProcess == true)
        {
            //r.velocity = new Vector2(0, 0);
            GainEnergy();
            //t.DrawTraject(transform.position, GetComponent<Rigidbody2D>().velocity, Input.mousePosition, propulseStrength);
        }

        if (propulseStrength >= propulseStrengthMax)
        {
            //Jump(Input.mousePosition);
            Propulse(Input.mousePosition);
            t.ClearTraject();
            loadingProcess = false;
            InitEnergy();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            ReactToGround(collision);
            //t.ClearTraject();
            //loadingProcess = false;
            //InitEnergy();
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("End jump");
            canJump = false;
        }
    }

    private void Jump(Vector2 click)
    {
        r.velocity = new Vector2(0, 0);
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 forceToApply = new Vector2(transform.position.x, transform.position.y) - clickToWorld;
        r.AddForce(forceToApply.normalized * strength, ForceMode2D.Impulse);
        jumped = true;
    }

    private void Rope(Vector2 click)
    {
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
    }

    private void Attach(Rigidbody2D ri)
    {
        if (isAttached == false)
        {
            gameObject.AddComponent<FixedJoint2D>();
            gameObject.GetComponent<FixedJoint2D>().enableCollision = true;
            gameObject.GetComponent<FixedJoint2D>().connectedBody = ri;
            isAttached = true;
            isImpulsing = false;
        }
        
    }

    private void Detach()
    {
        r.gravityScale = 1;
        isAttached = false;
        if (gameObject.GetComponent<FixedJoint2D>() != null)
        {
            Destroy(gameObject.GetComponent<FixedJoint2D>());
        }
    }

    private void Propulse(Vector2 click)
    {
        Debug.Log("Propulse !");
        isImpulsing = true;
        Detach();
        r.velocity = new Vector2(0, 0);
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 forceToApply = new Vector2(transform.position.x, transform.position.y) - clickToWorld;
        r.AddForce(forceToApply.normalized * propulseStrength, ForceMode2D.Impulse);
    }

    private void Propulse2(Vector2 click)
    {
        isImpulsing = true;
        r.velocity = new Vector2(0, 0);
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 forceToApply = new Vector2(transform.position.x, transform.position.y) - clickToWorld;
        r.AddForce(forceToApply * strength, ForceMode2D.Impulse);
    }

    private void GainEnergy()
    {
        if (propulseStrength < propulseStrengthMax)
        {
            propulseStrength += propulseStrengthSteps;
        }
    }

    private void InitEnergy()
    {
        propulseStrength = 0;
    }

    private void ReactToGround(Collision2D col)
    {
        jumped = false;
        isImpulsing = false;
        anim.SetFloat("velocity", r.velocity.magnitude);
        anim.SetTrigger("hit");
        switch (col.gameObject.name)
        {
            case "GrabableWall":
                ground = Ground.grabable;
                Attach(col.rigidbody);
                canJump = true;
                break;
            case "BumpyWall":
                ground = Ground.bumpy;
                r.velocity = new Vector2(0, 0);
                r.AddForce(new Vector2(col.gameObject.transform.position.x - transform.position.x, 0) * 100, ForceMode2D.Impulse);
                break;
            case "Wall":
                canJump = true;
                break;
            case "Turret":
                canJump = true;
                break;
            case "GrabableTurret":
                ground = Ground.grabable;
                Attach(col.rigidbody);
                canJump = true;
                break;

        }
    }



}
