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

    public override bool ReactToObstacle(Obstacle obstacle, Vector3 position)
    {
        if (base.ReactToObstacle(obstacle, position))
        {
            _jumpManager.GainAllJumps();
            return true;
        }

        return false;
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
            if (speedFactor == 0)
            {
                Rigidbody.velocity = Vector2.zero;
            }
            Debug.Log("Dir : " + _touchManager.GetWalkDirection() + " Normal : " + CollisionNormal + " Speed : " + CurrentSpeed);
            jointMotor.motorSpeed = speedFactor;
            hinge.motor = jointMotor;
            hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    public override void Attach(Obstacle obstacle)
    {
        base.Attach(obstacle);
        if (!_touchManager.Touching)
        {
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.angularVelocity = 0;
            Rigidbody.isKinematic = true;
        }
    }

    private float GetHeroSpeed(Vector3 direction, Vector3 platformNormal, float speed)
    {
        var dir = Vector3.Dot(direction, platformNormal);
        var sign = Mathf.Sign(dir);

        if (sign * dir > .5f)
            return sign * speed;

        return 0;
    }
}
