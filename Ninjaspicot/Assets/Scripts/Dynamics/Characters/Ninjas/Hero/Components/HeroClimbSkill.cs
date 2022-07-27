using System;
using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Characters.Hero.Components
{
    public class HeroClimbSkill : ClimbSkill
    {
        private ITouchService _touchService;
        private IAudioService _audioService;
        private AudioSource _audioSource;
        private AudioFile _walkAudio;
        private AudioFile _runAudio;
        private Vector3 _previousPosition;
        private Vector3 _walkDirection;

        public override void Awake()
        {
            base.Awake();

            _touchService = ServiceFinder.Get<ITouchService>();
            _audioService = ServiceFinder.Get<IAudioService>();
            _audioSource = GetComponent<AudioSource>();
            _walkAudio = _audioService.FindAudioByName("Whoosh3");
            _runAudio = _audioService.FindAudioByName("Whoosh2");
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

        protected override IEnumerator WalkOnWalls(HingeJoint2D hinge)
        {
            if (hinge == null)
                yield break;

            var jointMotor = hinge.motor;
            hinge.useMotor = true;
            var whooshing = false;

            while (true)
            {
                _speedFactor = GetHeroSpeed(_touchService.LeftDragDirection, CollisionNormal, CurrentSpeed);
                _walkDirection = _touchService.LeftDragDirection;
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
                hinge.anchor = Transform.InverseTransformPoint(GetContactPosition());

                yield return null;
            }
        }

        private IEnumerator Whoosh(AudioFile audio, float intensity, float time, Action<bool> callback)
        {
            _audioService.PlaySound(_audioSource, audio, intensity);

            yield return new WaitForSeconds(time);
            callback(false);
        }

        public override bool Attach(Obstacle obstacle)
        {
            if (!base.Attach(obstacle))
                return false;

            if (!_touchService.Touching)
            {
                Rigidbody.velocity = Vector2.zero;
                Rigidbody.angularVelocity = 0;
                Rigidbody.isKinematic = true;
            }

            return true;
        }

        private float GetHeroSpeed(Vector3 direction, Vector3 platformNormal, float speed)
        {
            var dir = Vector3.Dot(direction, platformNormal);
            var sign = Mathf.Sign(dir);

            var directionChange = (direction - _walkDirection).magnitude;
            //Keep old speed unless different direction
            if (_speedFactor != 0 && directionChange < .01f)
                return Mathf.Clamp(_speedFactor, -1, 1) * speed;

            return sign * speed;


            //For 2D direction
            //return direction.normalized.magnitude * Mathf.Sign(direction.x) * speed;
        }
    }
}