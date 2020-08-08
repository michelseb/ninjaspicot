using System.Collections;
using UnityEngine;

public class HeroStickiness : Stickiness
{
    private TouchManager _touchManager;

    public override void Awake()
    {
        base.Awake();
        _touchManager = TouchManager.Instance;
    }

    public override void ReactToObstacle(Obstacle obstacle, Vector3 position)
    {
        base.ReactToObstacle(obstacle, position);
        _jumpManager.GainAllJumps();
    }

    protected override IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;
        hinge.useMotor = true;

        while (true)
        {
            var speedFactor = GetHeroSpeed(_touchManager.GetWalkDirection(), CollisionNormal, CurrentSpeed);
            jointMotor.motorSpeed = speedFactor;
            hinge.motor = jointMotor;
            hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    private float GetHeroSpeed(Vector3 direction, Vector3 platformNormal, float speed)
    {
        var dir = Vector3.Dot(direction, platformNormal);
        
        if (dir > .1f) 
        {
            return speed;
        }
        else if (dir < -.1f)
        {
            return -speed;
        }

        return 0;
    }
}
