using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : Ninja {

    Trajectoire tr;
    private int bulletsColliding = 0;

    bool attacked;
    private IEnumerator zoom;

    private void Start()
    {
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
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == currCollider || collision.gameObject.tag == "Wall" || currCollider.tag == "Wall")
        {
            Debug.Log("Ok");
            d.isWalking = false;
            currCollider = null;
            d.Detach();
            d.LoseJump();
            if (d.readyToJump)
            {
                //StartCoroutine(tr.FadeAway());
            }
        }
    }

}
