using UnityEngine;

public class RepulsiveObstacle : Obstacle
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        var ninja = collision.gameObject.GetComponent<INinja>();
        
        if (ninja == null)
            return;

        ninja.Stickiness.Detach();
    }
}
