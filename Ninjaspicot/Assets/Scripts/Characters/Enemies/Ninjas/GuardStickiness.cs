using System.Collections;
using UnityEngine;

public class GuardStickiness : Stickiness
{
    protected Coroutine _walkTowards;
    protected GuardNinja _guard;

    public override void Awake()
    {
        base.Awake();
        _guard = GetComponent<GuardNinja>();
    }

    public virtual void StartWalkingTowards(LocationPoint location, Vector3 destination)
    {
        if (!Active)
            return;

        StopWalkingTowards(true);

        Rigidbody.isKinematic = false;
        _walkTowards = StartCoroutine(WalkTowards(WallJoint, location, destination));
    }

    public virtual void StopWalkingTowards(bool restart)
    {
        if (_walkTowards != null)
        {
            StopCoroutine(_walkTowards);
            _walkTowards = null;
        }

        if (Attached && !restart)
        {
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.angularVelocity = 0;
            WallJoint.useMotor = false;
        }
    }

    protected virtual IEnumerator WalkTowards(HingeJoint2D hinge, LocationPoint location, Vector3 destination)
    {
        if (hinge == null)
        {
            StopWalkingTowards(false);
            yield break;
        }

        CurrentSpeed = location.Id > LocationPoint.Id ? -_speed : _speed;

        var jointMotor = hinge.motor;
        jointMotor.motorSpeed = CurrentSpeed;
        hinge.motor = jointMotor;

        hinge.useMotor = true;

        float distToDestination;

        while (true)
        {
            hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());
            distToDestination = Vector3.Distance(_transform.position, destination);

            if (distToDestination < 2)
            {
                StopWalkingTowards(false);
                yield break;
            }

            yield return null;
        }
    }

    public override bool Attach(Obstacle obstacle)
    {
        if (!base.Attach(obstacle))
            return false;

        if (_guard.GuardMode == GuardMode.Chasing)
        {
            StartWalkingTowards(_guard.TargetLocation, _guard.Target);
        }

        return true;
    }
}
