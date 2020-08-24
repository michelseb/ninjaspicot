using UnityEngine;

public class DynamicStickiness : Stickiness
{
    private Hero _hero;

    public override void Awake()
    {
        base.Awake();
        _hero = Hero.Instance;
    }

    public override void Start()
    {
        base.Start();
        CurrentSpeed = _hero.Stickiness.CurrentSpeed;
    }


    public override bool ReactToObstacle(Obstacle obstacle, Vector3 contactPoint)
    {
        if (!Active || obstacle == CurrentAttachment)
            return false;

        _hero.Jumper.GainAllJumps();

        return base.ReactToObstacle(obstacle, contactPoint);
    }
}
