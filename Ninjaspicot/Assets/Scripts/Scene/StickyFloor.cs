using UnityEngine;

public class StickyFloor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var jumpManager = collision.GetComponent<JumpManager>();

        if (jumpManager == null)
            return;

        jumpManager.LoseAllJumps();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var jumpManager = collision.GetComponent<JumpManager>();

        if (jumpManager == null)
            return;

        jumpManager.GainAllJumps();
    }
}
