using UnityEngine;

public class ExtraJump : Bonus
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Jumper jumpManager))
        {
            jumpManager.GainJumps(1);
            _audioManager.PlaySound(_audioSource, "ExtraJump", .5f);
        }

        base.OnTriggerEnter2D(collision);
    }
}
