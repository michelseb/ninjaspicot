using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour, IDestructable {

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
        FixedJoint2D j = GetComponent<FixedJoint2D>();
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
            r.AddForce(killer.position - transform.position, ForceMode2D.Impulse);
            r.AddTorque(20, ForceMode2D.Impulse);
        }
        StartCoroutine(Dying());
    }

    public IEnumerator Dying()
    {
        t.RestoreTime();
        yield return new WaitForSeconds(2);
        ScenesManager.BackToCheckpoint();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            if (d != null)
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
