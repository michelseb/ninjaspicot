using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : MonoBehaviour, IDestructable {

    public ContactPoint2D contact, previousContact;
    public ContactPoint2D[] contacts;
    public Deplacement d;
    public Rigidbody2D r;
    public Camera c;
    public CameraBehaviour cam;
    public TimeManager t;
    public Queue<GameObject> lastColliders;
    public GameObject currCollider;
    Ghost g;
    private float highestPoint;
    bool selectMode;
    public bool getsCheckPoint;

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
        lastColliders = new Queue<GameObject>();

        if (getsCheckPoint)
        {
            ScenesManager.SetCheckpoint();
        }
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
        
        if (lastColliders.Count > 5)
        {
            lastColliders.Dequeue();
        }
        //r.velocity = currCollider.GetComponent<Rigidbody2D>().velocity;
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

    
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (d != null)
        {
            contact = SelectContactPoint(collision.contacts, previousContact);
            previousContact = contact;
        }
        contacts = collision.contacts;
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

    ContactPoint2D SelectContactPoint(ContactPoint2D[] contacts, ContactPoint2D previous) //WOOOOHOOO ça marche !!!!!
    {
        
        ContactPoint2D cont = new ContactPoint2D();
        float dist = 0;
        foreach (ContactPoint2D c in contacts)
        {
            if (Vector3.Distance(previous.point, c.point) > dist)
            {
                dist = Vector3.Distance(previous.point, c.point);
                cont = c;
            }
        }

        return cont;
    }

    public bool CheckRecentCollider(GameObject col)
    {
        if (col != currCollider)
        {
            if (lastColliders.Contains(col))
            {
                return false;
            }
        }
        return true;
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .4f);
        Gizmos.DrawSphere(contact.point, 1f);
    }*/
}
