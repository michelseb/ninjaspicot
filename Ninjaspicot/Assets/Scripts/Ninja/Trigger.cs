using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : Ninja {

    private int bulletsColliding = 0;
    ParticleSystem p;

    private void Awake()
    {
        p = GetComponent<ParticleSystem>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "bullet(Clone)")
        {
            if (d != null)
            {
                GetComponent<CapsuleCollider2D>().size = new Vector2(12, 16);
                StartCoroutine(cam.zoomIn(20));
                t.SlowDown();

            }

            bulletsColliding++;
            Debug.Log("Bullets : " + bulletsColliding);
        }
    }


    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            if (d != null)
            {
                Debug.Log("End jump");
                //d.LoseJump();
            }
        }

        if (collision.gameObject.name == "bullet(Clone)")
        {
            bulletsColliding--;
            Debug.Log("Bullets left : " + bulletsColliding);
            if (bulletsColliding == 0)
            {
                GetComponent<CapsuleCollider2D>().size = new Vector2(10, 14);
                StartCoroutine(cam.zoomOut(60));
                StartCoroutine(t.RestoreTime());
            }
        }
    }

    public void ActivateParticles()
    {
        p.Play();
    }

    public void DeactivateParticles()
    {
        p.Stop();
    }


}
