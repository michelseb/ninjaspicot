using System.Collections;
using UnityEngine;

public class DynamicStickiness : Stickiness
{
    private Hero _hero;

    public override void Awake()
    {
        base.Awake();
        _hero = Hero.Instance;
    }


    public override void ReactToObstacle(Obstacle obstacle)
    {
        if (!Active || obstacle == CurrentAttachment)
            return;

        _hero.JumpManager.GainAllJumps();

        base.ReactToObstacle(obstacle);
    }
}
