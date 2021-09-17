using System;
using System.Collections;
using UnityEngine;

public class HeroStickiness : Stickiness
{
    private TouchManager _touchManager;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private Audio _walkAudio;
    private Audio _runAudio;
    private Vector3 _previousPosition;
    private Vector3 _walkDirection;

    public override void Awake()
    {
        base.Awake();
        _touchManager = TouchManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _walkAudio = _audioManager.FindAudioByName("Whoosh3");
        _runAudio = _audioManager.FindAudioByName("Whoosh2");
    }

    public override void Start()
    {
        base.Start();
        _previousPosition = Transform.position;
    }

    public void LateUpdate()
    {
        if (!Attached && _previousPosition != Transform.position)
        {
            Transform.rotation = Quaternion.LookRotation(Vector3.forward, Transform.position - _previousPosition);
            _previousPosition = Transform.position;
        }
    }

    public override bool ReactToObstacle(Obstacle obstacle, Vector3 position)
    {
        if (base.ReactToObstacle(obstacle, position))
        {
            _jumper.GainAllJumps();

            //Sound effect
            //var intensity = (int)ImpactVelocity / 50;
            //_audioSource.PlayOneShot(_impact, intensity);
            //var soundEffect = _poolManager.GetPoolable<SoundEffect>(position, Quaternion.identity, intensity);
            //soundEffect.SetComposite(CurrentAttachment.Collider);

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
        var whooshing = false;

        while (true)
        {
            _speedFactor = GetHeroSpeed(_touchManager.GetWalkDirection(), CollisionNormal, CurrentSpeed);
            _walkDirection = _touchManager.GetWalkDirection();
            if (_speedFactor == 0)
            {
                Rigidbody.velocity = Vector2.zero;
            }
            else if (!whooshing)
            {
                var coroutine = Running ?
                    Whoosh(_runAudio, .07f, .13f, callback => { whooshing = callback; }) :
                    Whoosh(_walkAudio, .04f, .26f, callback => { whooshing = callback; });

                StartCoroutine(coroutine);
                whooshing = true;
            }

            jointMotor.motorSpeed = _speedFactor;
            hinge.motor = jointMotor;
            hinge.anchor = _transform.InverseTransformPoint(GetContactPosition());

            yield return null;
        }
    }

    private IEnumerator Whoosh(Audio audio, float intensity, float time, Action<bool> callback)
    {
        _audioManager.PlaySound(_audioSource, audio, intensity);

        yield return new WaitForSeconds(time);
        callback(false);
    }

    public override bool Attach(Obstacle obstacle)
    {
        if (!base.Attach(obstacle))
            return false;

        if (!_touchManager.Touching)
        {
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.angularVelocity = 0;
            Rigidbody.isKinematic = true;
        }

        return true;
    }

    //public override void Detach()
    //{
    //    if (!Attached)
    //        return;

    //    CurrentAttachment.LaunchQuickDeactivate();
    //    base.Detach();
    //}

    private float GetHeroSpeed(Vector3 direction, Vector3 platformNormal, float speed)
    {
        var dir = Vector3.Dot(direction, platformNormal);
        var sign = Mathf.Sign(dir);

        var directionChange = (direction - _walkDirection).magnitude;
        //Keep old speed unless different direction
        if (_speedFactor != 0 && directionChange < .01f)
            return _speedFactor;

        Debug.Log("Regular");
        return sign * speed;


        //For 2D direction
        //return direction.normalized.magnitude * Mathf.Sign(direction.x) * speed;
    }
}
