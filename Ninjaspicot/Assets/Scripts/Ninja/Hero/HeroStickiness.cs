using UnityEngine;

public class HeroStickiness : Stickiness
{
    public override void ReactToObstacle(Obstacle obstacle, Vector3 position, bool localContact)
    {
        base.ReactToObstacle(obstacle, position, localContact);
        _jumpManager.GainAllJumps();
    }
}
