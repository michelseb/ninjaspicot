using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    private GameObject ninja;
    public GameObject canon;
    public GameObject bullet;
    [SerializeField]
    private float strength, loadProgress;
    [SerializeField]
    private float loadTime;
    private bool loaded = true;
    private RaycastHit2D hit;
    Vector3 dir;
    private Coroutine search;
    private enum Mode {Scan, Aim, Wait};
    Mode turretMode;
    
    

    // Use this for initialization
    void Start () {

        ninja = GameObject.Find("Ninjaspicot");
        turretMode = Mode.Scan;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        dir = (bullet.transform.position - transform.position);

        switch (turretMode)
        {
            case Mode.Aim:
                
                if (loaded)
                {
                    Shoot();
                    StartCoroutine(Load());
                    loaded = false;
                }

                break;

            case Mode.Scan:

                transform.Rotate(0, 0, 2);
                break;

            case Mode.Wait:

                Vector3 dest = ninja.transform.position - bullet.transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, dest), .1f);

                break;
        }

        
        hit = Physics2D.Raycast(bullet.transform.position, dir, 1000f, LayerMask.GetMask("Default"));
        Debug.DrawRay(bullet.transform.position, dir * 10, Color.green);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "ninja")
            {
                if (turretMode == Mode.Wait)
                {
                    StopCoroutine(search);
                }
                turretMode = Mode.Aim;

            }
            else
            {
                if (turretMode == Mode.Aim)
                {
                    turretMode = Mode.Wait;
                    search = StartCoroutine(Search());
                }

            }
        } else
        {
            if (turretMode == Mode.Aim)
            {
                turretMode = Mode.Wait;
                search = StartCoroutine(Search());
            }
        }

       

	}

    private void Shoot()
    {
        
        GameObject b = Instantiate(bullet, bullet.transform.position + dir/2, Quaternion.identity);
        b.transform.parent = null;
        b.AddComponent<Rigidbody2D>();
        b.AddComponent<Bullet>();
        b.GetComponent<Rigidbody2D>().gravityScale = 0;
        b.GetComponent<Rigidbody2D>().AddForce( dir * strength, ForceMode2D.Impulse);
    }
    private IEnumerator Load()
    {
        while (loadProgress < loadTime){
            loadProgress += .1f;
            yield return null;

        }
        loaded = true;
        loadProgress = 0;
        
    }

    private IEnumerator Search()
    {
        yield return new WaitForSeconds(3);
        if (turretMode != Mode.Aim)
        {
            turretMode = Mode.Scan;
        }
        
    }

}
