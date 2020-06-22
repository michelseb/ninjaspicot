using UnityEngine;

public class HeroStickiness : Stickiness
{
    public override void ReactToObstacle(Obstacle obstacle, Vector3 position)
    {
        base.ReactToObstacle(obstacle, position);
        _jumpManager.GainAllJumps();
    }
}
