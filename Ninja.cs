using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour, IDestructable {

    public ContactPoint2D contact;
    public Deplacement d;
    public Rigidbody2D r;
    public Camera c;
    public CameraBehaviour cam;
    public TimeManager t;
    Ghost g;
    private float highestPoint;
    bool selectMode;
	// Use this for initialization
    void Awake()
    {
        //SelectDeplacementMode();
    }
	void Start () {
        d = FindObjectOfType<Deplacement>();
        r = GetComponent<Rigidbody2D>();
        c = FindObjectOfType<Camera>();
        cam = FindObjectOfType<CameraBehaviour>();
        t = FindObjectOfType<TimeManager>();
        g = FindObjectOfType<Ghost>();
        t.activated = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (d == null)
        {
            d = FindObjectOfType<Deplacement>();
        }
        
        if (transform.position.y > highestPoint)
        {
            highestPoint = transform.position.y;
        }
    }

    public void Die(Transform killer)
    {
        g.saveAll(highestPoint);
        Destroy(d);
        HingeJoint2D j = GetComponent<HingeJoint2D>();
        if (j != null)
        {
            Destroy(j);
        }
        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (killer != null)
        {
            if (killer.GetComponent<SpriteRenderer>() != null)
            {
                killer.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
            r.AddForce(killer.position - transform.position, ForceMode2D.Impulse);
            r.AddTorque(20, ForceMode2D.Impulse);
        }
        StartCoroutine(Dying());
    }

    public IEnumerator Dying()
    {
        t.NormalTime();
        t.activated = false;
        yield return new WaitForSeconds(2);
        ScenesManager.BackToCheckpoint();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (d != null)
        {
            if (d.hinge != null)
            {
                if (d.hinge.motor.motorSpeed > 0)
                {
                    contact = collision.contacts[collision.contacts.Length - 1];
                }
                else
                {
                    contact = collision.contacts[0];
                }
            }
        }
        
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (d != null)
                d.ReactToGround(collision);
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (d != null)
        {
            if (d.hinge != null)
            {
                if (d.hinge.motor.motorSpeed > 0)
                {
                    contact = collision.contacts[collision.contacts.Length - 1];
                }
                else
                {
                    contact = collision.contacts[0];
                }
            }
        }

        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (d != null && d.isAttached != false)
                d.ReactToGround(collision);
        }
    }



    private void SelectDeplacementMode()
    {
        selectMode = true;

    }

    void OnGUI()
    {
        if (selectMode)
        {
            if (GUI.Button(new Rect(0, Screen.height/2 - 20, Screen.width/2, 40), "Discrete mode"))
            {
                gameObject.AddComponent<DeplacementPrecis>();
                selectMode = false;

            }
            else if(GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 - 20, Screen.width / 2, 40), "Furtive mode"))
            {
                gameObject.AddComponent<DeplacementFurtif>();
                selectMode = false;
            }  
        }
    }

}
