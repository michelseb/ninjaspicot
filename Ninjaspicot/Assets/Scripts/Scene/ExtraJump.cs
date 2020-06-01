using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var movement = collision.GetComponent<Movement>() ?? collision.GetComponentInParent<Movement>();
        if (movement != null)
        {
            movement.GainJumps(1);
        }

        Destroy(gameObject);
    }
}
