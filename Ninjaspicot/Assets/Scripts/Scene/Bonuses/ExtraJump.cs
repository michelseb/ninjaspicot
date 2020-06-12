using UnityEngine;

public class ExtraJump : Bonus
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        var jumpManager = collision.GetComponent<Jumper>();
        if (jumpManager != null)
        {
            jumpManager.GainJumps(1);
        }

        base.OnTriggerEnter2D(collision);
    }
}
