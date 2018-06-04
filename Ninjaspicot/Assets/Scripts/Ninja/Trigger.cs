using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour {

    Trajectoire tr;
    public Camera c;
    public CameraBehaviour cam;
    Ninja n;
    Deplacement d;
    TimeManager t;
    Touch to;
    private int bulletsColliding = 0;

    bool attacked;
    private IEnumerator zoom;

    private void Start()
    {
        to = FindObjectOfType<Touch>();
        d = FindObjectOfType<Deplacement>();
        c = FindObjectOfType<Camera>();
        n = FindObjectOfType<Ninja>();
        t = FindObjectOfType<TimeManager>();
        cam = FindObjectOfType<CameraBehaviour>();
        tr = c.gameObject.GetComponent<Trajectoire>();
    }

    private void Update()
    {
        if (bulletsColliding <= 0 && attacked)
        {
            GetComponent<CapsuleCollider2D>().size = new Vector2(10, 14);
            if (zoom != null)
            {
                StopCoroutine(zoom);
            }
            zoom = cam.zoomOut(0);
            StartCoroutine(zoom);
            StartCoroutine(t.RestoreTime());
            attacked = false;
        }
    }

    private void FixedUpdate()
    {
        bulletsColliding = 0;
    }

    

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "bullet(Clone)")
        {
            bulletsColliding++;
            attacked = true;
        }

        if (collision.gameObject.tag == "Wall")
        {
            if (n.HasBeenCurrentCollider(collision.gameObject))
            {
                if (d.isAttached)
                {
                    d.isWalking = false;
                    d.Detach();
                }
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "bullet(Clone)")
        {
            if (d != null && d.GetJumps() > 0)
            {
                GetComponent<CapsuleCollider2D>().size = new Vector2(12, 16);
                zoom = cam.zoomIn(20);
                StartCoroutine(zoom);
                //t.SlowDown(.03f);

            }
        }

        if (collision.gameObject.tag == "Wall")
        {
            if (n.HasBeenCurrentCollider(collision.gameObject))
            {
                if (d.readyToJump)
                {
                    StartCoroutine(tr.FadeAway());
                }
                d.isWalking = false;
                d.Detach();
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (n.currCollider != null)
        {
            if (collision.gameObject == n.currCollider || collision.gameObject.tag == "Wall" || n.currCollider.tag == "Wall")
            {
                if (n.HasBeenCurrentCollider(collision.gameObject))
                {
                    if (d.readyToJump)
                    {
                        StartCoroutine(tr.FadeAway());
                    }
                    d.isWalking = false;
                    d.Detach();
                    d.LoseJump();
                }

            }
        }
    }

}
