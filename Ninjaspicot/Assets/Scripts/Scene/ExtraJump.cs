using UnityEngine;

public class ExtraJump : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var jumpManager = collision.GetComponent<JumpManager>() ?? collision.GetComponentInParent<JumpManager>();
        if (jumpManager != null)
        {
            jumpManager.GainJumps(1);
        }

        Destroy(gameObject);
    }
}
