using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Deplacement : MonoBehaviour {

    public Trajectoire t;
    public TimeManager time;
    public CameraBehaviour cam;
    public Rigidbody2D r;
    public Camera c;
    public float strength;
    public bool isAttached, preciseJump, isSticking;
    Animator anim;
    private enum Ground { grabable, bumpy };
    private int numberOfJumpsAllowed; 
    public int maxNumberOfJumpsAllowed;
    Ground ground;
    public Vector2 originClic;
    public JointMotor2D motor;
    public HingeJoint2D hinge;
    public Ninja n;
    public int rapidite, OriginalRapidite;
    public bool readyToJump, isWalking;
    public bool started;



    public enum Dir
    {
        Left,
        Right
    }

    public Dir ninjaDir;

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
        
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
        Vector2 originClickToWorld = c.ScreenToWorldPoint(new Vector3(originClic.x, originClic.y, 0));
        Vector2 forceToApply = originClickToWorld - clickToWorld;
        isSticking = false;
        LoseJump();
        Detach();
        StartCoroutine(t.FadeAway());
        r.velocity = new Vector2(0, 0);
        if (GetJumps() <= 0)
        {
            StartCoroutine(time.RestoreTime());
            cam.zoomOut(0);

        }
            
        r.AddForce(forceToApply.normalized * strength /** (GetJumps() + 1) / GetMaxJumps()*/, ForceMode2D.Impulse);
        
    }

    private void Rope(Vector2 click)
    {
        Vector2 clickToWorld = c.ScreenToWorldPoint(new Vector3(click.x, click.y, 0));
    }

    public void Attach(Rigidbody2D ri)
    {
        if (isAttached == false)
        {
            r.velocity = new Vector2(0, 0);
            hinge = gameObject.AddComponent<HingeJoint2D>();
            hinge.useLimits = false;
            hinge.anchor = transform.InverseTransformPoint(n.contact.point);
            hinge.connectedAnchor = transform.InverseTransformPoint(n.contact.point);
            hinge.enableCollision = true;
            if (ri != null)
            {
                hinge.connectedBody = ri;
            }
            hinge.useMotor = true;
            JointMotor2D motor = hinge.motor;
            hinge.motor = motor;
            isSticking = true;
        }
        isAttached = true;

    }

    public IEnumerator SpeedBoost(float time)
    {
        rapidite *= 2;
        yield return new WaitForSeconds(time);
        rapidite = OriginalRapidite;
    }


    public void Detach()
    {
        Debug.Log("Detach");
        isAttached = false;
        if (gameObject.GetComponent<HingeJoint2D>() != null)
        {
            Destroy(gameObject.GetComponent<HingeJoint2D>());
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

    public void ReactToGround(GameObject g)
    {
        GainAllJumps();
        anim.SetFloat("velocity", r.velocity.magnitude);
        anim.SetTrigger("hit");
        ground = Ground.grabable;
        if (g.tag != "Wall")
        {
            Attach(g.GetComponent<Rigidbody2D>());
        }
    }



}
