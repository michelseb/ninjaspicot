using System.Collections;
using UnityEngine;

public class ParkingDoor : MonoBehaviour {

    private GameObject _pivot;
    private bool _moveUp;
    private Coroutine _down;
    

    private void Update()
    {
        if (_pivot.transform.rotation.z > 0 && _moveUp == false)
        {
            _pivot.transform.Rotate(0, 0, -60*Time.deltaTime);//transform.Translate(0, -5*Time.deltaTime, 0);
        }

        if (_moveUp && _pivot.transform.rotation.z < .7f)
        {
            _pivot.transform.Rotate(0, 0, 60 * Time.deltaTime);// (0, 20*Time.deltaTime, 0);
        }

        if (_moveUp && _pivot.transform.rotation.z > .7f)
        {
            _down = StartCoroutine(goDown());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "auto")
        {
            if (_pivot.transform.rotation.z < .7f)
            {
                if (_down != null)
                {
                    StopCoroutine(_down);
                }
                _moveUp = true;
            }
        }
    }

    IEnumerator goDown()
    {
        yield return new WaitForSeconds(2);
        _moveUp = false;
    }

}
