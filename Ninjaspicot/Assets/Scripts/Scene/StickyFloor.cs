using UnityEngine;

public class StickyFloor : MonoBehaviour {

    private Movement _movement;

    private void Start()
    {
        _movement = FindObjectOfType<Movement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            if (_movement.GetJumps() > 0)
            {
                _movement.LoseJump();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ninja")
        {
            _movement.GainAllJumps();
        }
    }
}
