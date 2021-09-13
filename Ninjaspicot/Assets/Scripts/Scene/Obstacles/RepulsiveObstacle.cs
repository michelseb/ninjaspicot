using UnityEngine;

public class RepulsiveObstacle : Obstacle
{
    //private bool _isHeroAttached;

    //protected override void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!collision.collider.CompareTag("hero"))
    //        return;

    //    _isHeroAttached = true;
    //}

    //protected virtual void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (!collision.collider.CompareTag("hero"))
    //        return;

    //    _isHeroAttached = false;
    //}

    public void DetachHero()
    {
        var stickiness = Hero.Instance.Stickiness;
        if (stickiness.CurrentAttachment == this)
        {
            stickiness.Detach();
        }
    }
}
