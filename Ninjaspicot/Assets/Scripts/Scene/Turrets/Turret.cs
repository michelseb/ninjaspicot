using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    public bool autoShoot;
    public float rotationAngle;
    public float rotationSpeed;
    private int sens = 1;
    private float initRotationAngle;
    private GameObject ninja;
    private Deplacement ninjaScript;
    public GameObject canon;
    public GameObject bullet;
    private Renderer r;
    [SerializeField]
    private float strength, loadProgress;
    [SerializeField]
    private float loadTime;
    private bool loaded = true;
    Vector3 dir;
    private Coroutine search;
    public enum Mode {Scan, Aim, Wait};
    public int vision;
    public Mode turretMode;
    Vector3 dest;
    public int facteur;


    // Use this for initialization
    void Start () {

        facteur = 1;
        ninja = GameObject.Find("Ninjaspicot");
        ninjaScript = FindObjectOfType<Deplacement>();
        turretMode = Mode.Scan;
        r = GetComponent<Renderer>();
        initRotationAngle = transform.rotation.z;
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);
        dir = (bullet.transform.position - transform.position);

        switch (turretMode)
        {
            case Mode.Aim:

                dest = ninja.transform.position - bullet.transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, dest), .1f);
                if (loaded)
                {
                    Shoot();
                    StartCoroutine(Load());
                    loaded = false;
                }

                break;

            case Mode.Scan:
                if (autoShoot)
                {
                    if (loaded)
                    {
                        Shoot();
                        StartCoroutine(Load());
                        loaded = false;
                    }
                }
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime * sens * facteur);
                if (Mathf.Abs(transform.rotation.z - initRotationAngle) > rotationAngle)
                {
                    sens = -sens;
                }
                    break;

            case Mode.Wait:

                dest = ninja.transform.position - bullet.transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, dest), .1f);

                break;
        }

        
        /*if (r.isVisible == false)
        {
            turretMode = Mode.Scan;
        }*/

       

	}

    private void OnDrawGizmos()
    {
       
    }

    public void SelectMode(string evenement)
    {
        switch (evenement) {
            case "aim":
                if (turretMode == Mode.Wait)
                {
                    StopCoroutine(search);
                    turretMode = Mode.Aim;
                }
                else if (turretMode == Mode.Scan)
                {
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
                break;

            case "search":
                if (turretMode == Mode.Aim)
                {
                    turretMode = Mode.Wait;
                    search = StartCoroutine(Search());
                }
                break;
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
            loadProgress += 10* Time.deltaTime;
            yield return null;

        }
        loaded = true;
        loadProgress = 0;
        
    }

    private IEnumerator Search()
    {
        yield return new WaitForSeconds(2);
        if (turretMode != Mode.Aim)
        {
            turretMode = Mode.Scan;
        }
        
    }

}