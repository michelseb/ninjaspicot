using System.Collections;
using UnityEngine;

public class HeroStickiness : Stickiness
{
    //private TouchManager _touchManager;

    //public override void Awake()
    //{
    //    base.Awake();
    //    _touchManager = TouchManager.Instance;
    //}

    public override void ReactToObstacle(Obstacle obstacle, Vector3 position)
    {
        base.ReactToObstacle(obstacle, position);
        _jumpManager.GainAllJumps();
    }

    //protected override IEnumerator WalkOnWalls(HingeJoint2D hinge)
    //{
    //    if (hinge == null)
    //        yield break;

    //    var jointMotor = hinge.motor;
    //    hinge.useMotor = true;

    //    while (true)
    //    {
    //        var speedVector = _touchManager.Touch1Origin - transform.position;
    //        jointMotor.motorSpeed = speedVector.magnitude * Vector3.SignedAngle(Vector3.up, speedVector, Vector3.up);
    //        hinge.motor = jointMotor;
    //        hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());

    //        yield return null;
    //    }
    //}
}
