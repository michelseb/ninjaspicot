using UnityEngine;

public class DoorButton : MonoBehaviour {

    private bool _active;
    private Door _door;
    private Material _material;

	private void Start ()
    {
        _door = transform.GetComponentInParent<Door>();
        _material = GetComponent<SpriteRenderer>().material;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            if (_active)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }


    private void Open()
    {
        _active = true;
        _material.color = Color.green;
        _door.SetActive(true);
    }

    private void Close()
    {
        _active = false;
        _material.color = Color.red;
        _door.SetActive(false);
    }
}
