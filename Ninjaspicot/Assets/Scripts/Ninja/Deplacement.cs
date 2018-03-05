using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Deplacement : MonoBehaviour {

    public Trajectoire t;
    TimeManager time;
    CameraBehaviour cam;
    public Rigidbody2D r;
    public Camera c;
    public float strength;
    public bool canJump, jumped, canAttach, isAttached, preciseJump;
    Animator anim;
    private enum Ground { grabable, bumpy };
    private int numberOfJumpsAllowed, maxNumberOfJumpsAllowed;
    Ground ground;
    public Vector2 originClic;
    

    private void Awake()
    {
        r = this.GetComponent<Rigidbody2D>();
        c = GameObject.Find("Camera").GetComponent<Camera>();
        cam = c.GetComponent<CameraBehaviour>();
        t = c.gameObject.GetComponent<Trajectoire>();
        time = FindObjectOfType<TimeManager>();
        anim = GetComponent<Animator>();
        maxNumberOfJumpsAllowed = 4;
        numberOfJumpsAllowed = maxNumberOfJumpsAllowed;
        
    }


    public virtual void Jump(Vector2 click, float strength)
    {
        LoseJump();
        Detach();
        StartCoroutine(t.FadeAway());
        r.velocity = new Vector2(0, 0);
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 originClickToWorld = c.ScreenToWorldPoint(new Vector3(originClic.x, originClic.y, 0));
        Vector2 forceToApply = originClickToWorld - clickToWorld;
        if (GetJumps() <= 0)
        {
            StartCoroutine(time.RestoreTime());
            cam.zoomOut(60);
            
        }
        /*if (forceToApply.magnitude < 2)
        {
            if (click.x < Screen.width / 3)
            {
                r.AddForce((Vector2.up - Vector2.left) * strength/2, ForceMode2D.Impulse);
            }
            else if (click.x > Screen.width / 3 && click.x < 2 * Screen.width / 3)
            {
                r.AddForce(Vector2.up * strength / 2, ForceMode2D.Impulse);
            }
            else
            {
                r.AddForce((Vector2.up + Vector2.left) * strength / 2, ForceMode2D.Impulse);
            }
        }
        else
        {*/
        r.AddForce(forceToApply.normalized * strength * (GetJumps()+1)/GetMaxJumps(), ForceMode2D.Impulse);
        //}
        
    }

    private void Rope(Vector2 click)
    {
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
    }

    public void Attach(Rigidbody2D ri)
    {
        if (canAttach && isAttached == false)
        {
            gameObject.AddComponent<FixedJoint2D>();
            gameObject.GetComponent<FixedJoint2D>().enableCollision = true;
            gameObject.GetComponent<FixedJoint2D>().connectedBody = ri;
            
        }
        isAttached = true;

    }

    public void Detach()
    {
        isAttached = false;
        if (gameObject.GetComponent<FixedJoint2D>() != null)
        {
            Destroy(gameObject.GetComponent<FixedJoint2D>());
        }
    }

    public void LoseJump()
    {
        numberOfJumpsAllowed--;
    }


    public int GetJumps()
    {
        return numberOfJumpsAllowed;
    }

    public int GetMaxJumps()
    {
        return maxNumberOfJumpsAllowed;
    }

    public void SetMaxJumps(int amount)
    {
        maxNumberOfJumpsAllowed = amount;
    }

    public void SetJumps(int amount)
    {
        numberOfJumpsAllowed = amount;
    }

    public void GainAllJumps()
    {
        numberOfJumpsAllowed = maxNumberOfJumpsAllowed;
    }

    public void ReactToGround(Collision2D col)
    {
        jumped = false;
        GainAllJumps();
        anim.SetFloat("velocity", r.velocity.magnitude);
        anim.SetTrigger("hit");
        switch (col.gameObject.tag)
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
