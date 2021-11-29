using System.Collections;
using UnityEngine;

public class DynamicStickiness : Stickiness
{
    private Hero _hero;
    private TouchManager _touchManager;

    public override void Awake()
    {
        base.Awake();
        _hero = Hero.Instance;
        _touchManager = TouchManager.Instance;
        
    }

    public override void Start()
    {
        base.Start();
        CurrentSpeed = _hero.Stickiness.CurrentSpeed;
    }

    protected override IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;
        hinge.useMotor = true;
        //var walkAudio = _audioManager.FindByName("Whoosh3");
        //var runAudio = _audioManager.FindByName("Whoosh2");
        var whooshing = false;

        while (true)
        {
            var speedFactor = GetHeroSpeed(_touchManager.GetWalkDirection(), CollisionNormal, CurrentSpeed);
            if (speedFactor == 0)
            {
                Rigidbody.velocity = Vector2.zero;
            }
            else if (!whooshing)
            {
                //var coroutine = Running ?
                //    Whoosh(runAudio, .07f, .13f, callback => { whooshing = callback; }) :
                //    Whoosh(walkAudio, .04f, .26f, callback => { whooshing = callback; });

                //StartCoroutine(coroutine);
                //whooshing = true;
            }

            jointMotor.motorSpeed = speedFactor;
            hinge.motor = jointMotor;
            hinge.anchor = Transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    private float GetHeroSpeed(Vector3 direction, Vector3 platformNormal, float speed)
    {
        var dir = Vector3.Dot(direction, platformNormal);
        var sign = Mathf.Sign(dir);

        if (sign * dir > .3f)
            return sign * speed;

        return 0;
    }
}
