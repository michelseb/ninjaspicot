using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingDoor : MonoBehaviour {

    public GameObject pivot;
    public bool moveUp;
    Coroutine down;
    

    private void Update()
    {
        if (pivot.transform.rotation.z > 0 && moveUp == false)
        {
            pivot.transform.Rotate(0, 0, -60*Time.deltaTime);//transform.Translate(0, -5*Time.deltaTime, 0);
        }

        if (moveUp && pivot.transform.rotation.z < .7f)
        {
            pivot.transform.Rotate(0, 0, 60 * Time.deltaTime);// (0, 20*Time.deltaTime, 0);
        }

        if (moveUp && pivot.transform.rotation.z > .7f)
        {
            down = StartCoroutine(goDown());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "auto")
        {
            if (pivot.transform.rotation.z < .7f)
            {
                if (down != null)
                {
                    StopCoroutine(down);
                }
                moveUp = true;
            }
        }
    }

    IEnumerator goDown()
    {
        yield return new WaitForSeconds(2);
        moveUp = false;
    }

}
