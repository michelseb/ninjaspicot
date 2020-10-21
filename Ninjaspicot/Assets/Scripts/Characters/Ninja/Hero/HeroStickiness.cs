using System;
using System.Collections;
using UnityEngine;

public class HeroStickiness : Stickiness
{
    private TouchManager _touchManager;
    private PoolManager _poolManager;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private AudioClip _impact;
    public override void Awake()
    {
        base.Awake();
        _touchManager = TouchManager.Instance;
        _poolManager = PoolManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _impact = _audioManager.FindByName("Blob");
    }

    public override bool ReactToObstacle(Obstacle obstacle, Vector3 position)
    {
        if (base.ReactToObstacle(obstacle, position))
        {
            _jumper.GainAllJumps();

            //Sound effect
            var intensity = (int)ImpactVelocity / 50;
            _audioSource.PlayOneShot(_impact, intensity);
            var soundEffect = _poolManager.GetPoolable<SoundEffect>(position, Quaternion.identity, intensity);
            soundEffect.SetComposite(CurrentAttachment.Collider);

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
        var walkAudio = _audioManager.FindByName("Whoosh3");
        var runAudio = _audioManager.FindByName("Whoosh2");
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
                var coroutine = Running ?
                    Whoosh(runAudio, .07f, .13f, callback => { whooshing = callback; }) :
                    Whoosh(walkAudio, .04f, .26f, callback => { whooshing = callback; });

                StartCoroutine(coroutine);
                whooshing = true;
            }

            jointMotor.motorSpeed = speedFactor;
            hinge.motor = jointMotor;
            hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    private IEnumerator Whoosh(AudioClip audio, float intensity, float time, Action<bool> callback)
    {
        _audioSource.PlayOneShot(audio, intensity);
        yield return new WaitForSeconds(time);
        callback(false);
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

    public override void Detach()
    {
        if (!Attached)
            return;

        CurrentAttachment.LaunchQuickDeactivate();
        base.Detach();
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
