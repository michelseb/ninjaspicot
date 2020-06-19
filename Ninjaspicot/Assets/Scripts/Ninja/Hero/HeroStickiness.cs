using System.Collections;
using UnityEngine;

public class HeroStickiness : Stickiness
{
    private Hero _hero;

    public override void Awake()
    {
        base.Awake();

        _hero = _ninja as Hero;
    }


    public override void Attach(Obstacle obstacle)
    {
        //_hero.StopDisplayGhosts();
        base.Attach(obstacle);
    }

    public override void StartWalking()
    {
        //_hero.StartDisplayGhosts();
        base.StartWalking();
    }

    protected override IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        yield return base.WalkOnWalls(hinge);
        //_hero.StopDisplayGhosts();
    }
}
