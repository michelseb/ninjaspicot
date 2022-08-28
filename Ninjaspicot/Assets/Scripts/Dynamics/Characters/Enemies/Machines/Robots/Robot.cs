using System;
using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Hearing;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Components;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots
{
    public abstract class Robot : Enemy, IListener, IViewer, ISmart
    {
        [SerializeField] protected Transform _head;
        [SerializeField] protected float _hearingRange;
        [SerializeField] protected float _checkWonderTime;
        [SerializeField] protected float _returnWonderTime;

        protected HearingPerimeter _hearingPerimeter;
        protected Rigidbody2D _rigidbody;
        protected AudioFile _reactionSound;
        //protected Quaternion _initRotation;
        protected float _wonderTime;
        protected float _wonderElapsedTime;
        protected float _delayBetweenActions = .3f;
        protected float _remainingTimeBeforeAction;
        private Guid _movementId;

        protected ITimeService _timeService;
        protected ICoroutineService _coroutineService;

        protected Aim _aim;
        public Aim Aim { get { if (BaseUtils.IsNull(_aim)) _aim = GetComponentInChildren<Aim>(); return _aim; } }

        protected RobotLaser _laser;
        public RobotLaser Laser { get { if (BaseUtils.IsNull(_laser)) _laser = GetComponentInChildren<RobotLaser>(); return _laser; } }

        public MovementType CurrentMovement { get; protected set; }
        public ISeeable CurrentTarget { get; protected set; }
        public Transform TargetTransform => CurrentTarget?.Transform;
        public Vector3 TargetPosition { get; protected set; }

        public float Range => _hearingRange;
        public bool Seeing { get; set; }

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody2D>();
            _hearingPerimeter = GetComponentInChildren<HearingPerimeter>();
            _timeService = ServiceFinder.Get<ITimeService>();
            _coroutineService = ServiceFinder.Get<ICoroutineService>();
            _remainingTimeBeforeAction = _delayBetweenActions;
        }

        protected override void Start()
        {
            base.Start();
            _reactionSound = _audioService.FindByName("RobotReact");
            //_initRotation = _head.rotation;

            Laser?.Deactivate(null);
        }

        protected virtual void Update()
        {
            if (!Active)
                return;

            var rot = _head.localRotation.eulerAngles.z % 360;
            Renderer.flipY = rot >= 0 && rot < 180;

            //HandleState(State.StateType);
        }

        //#region Handlers
        //protected virtual void HandleState(StateType stateType)
        //{
        //    switch (stateType)
        //    {
        //        case StateType.Guard:
        //            Guard();
        //            break;
        //        case StateType.Patrol:
        //            Patrol();
        //            break;

        //        case StateType.Wonder:
        //            Wonder();
        //            break;

        //        case StateType.Check:
        //            Check();
        //            break;

        //        case StateType.Chase:
        //            Chase(TargetTransform.position);
        //            break;

        //        case StateType.Return:
        //            Return();
        //            break;

        //        case StateType.LookFor:
        //            LookFor();
        //            break;
        //    }
        //}
        //#endregion

        //protected virtual void StartSleeping()
        //{
        //    Laser?.Deactivate(null);
        //    Aim?.Deactivate(null);
        //}

        //protected virtual void StartWondering(StateType nextState)
        //{
        //    if (!IsState(StateType.Chase))
        //    {
        //        _audioService.PlaySound(_audioSource, _reactionSound, .4f);
        //    }

        //    Aim?.Activate();
        //    SetNextState(nextState);
        //    _wonderTime = GetReactionFactor(_initState);
        //    Renderer.color = ColorUtils.Red;
        //    _wonderElapsedTime = 0;
        //}


        //protected virtual void StartChecking()
        //{
        //    if (IsNextState(StateType.Check))
        //    {
        //        _initRotation = Transform.rotation;
        //    }

        //    SetNextState(StateType.Return);
        //    Renderer.color = ColorUtils.Red;
        //    Aim?.Activate();
        //}

        //protected virtual void StartChasing(ISeeable target)
        //{
        //    if (target == default)
        //        return;

        //    if (IsState(StateType.Guard))
        //    {
        //        _initRotation = Transform.rotation;
        //    }

        //    CurrentTarget = target;
        //    Renderer.color = ColorUtils.Red;
        //    Laser?.SetActive(true);
        //    Aim?.Activate();
        //}

        //protected virtual void StartLookFor()
        //{
        //    Renderer.color = ColorUtils.Yellow;
        //    Aim?.Activate();
        //}

        //protected virtual void StartReturning()
        //{
        //    CurrentTarget = default;
        //    Renderer.color = ColorUtils.White;
        //    Laser?.SetActive(false);
        //    Aim?.Activate();
        //}

        //protected virtual void StartGuarding()
        //{
        //    Renderer.color = ColorUtils.White;
        //    Laser?.SetActive(false);
        //    Aim?.Activate();
        //}

        //protected virtual void StartPatrolling()
        //{
        //    Renderer.color = ColorUtils.White;
        //    Laser?.SetActive(false);
        //    Aim?.Activate();
        //}


        //protected virtual void Wonder()
        //{
        //    _wonderElapsedTime += Time.deltaTime;

        //    if (_wonderElapsedTime >= _wonderTime)
        //    {
        //        SetState(State.NextState);
        //    }
        //}

        //protected abstract void Guard();
        //protected abstract void Patrol();
        //protected abstract void Check();
        //protected abstract void Chase(Vector3 targetPosition);
        //protected abstract void Return();
        //protected abstract void LookFor();

        protected abstract MovementType GetPatrolMovementType();
        protected abstract IEnumerator MoveTo(Vector3 target);
        protected abstract IEnumerator RotateTo(Vector3 target);
        protected abstract IEnumerator LaunchMovement(MovementType movementType);

        protected virtual IEnumerator ExecuteNextMovement()
        {
            CurrentMovement = GetPatrolMovementType();

            if (CurrentMovement == MovementType.None)
                yield break;

            yield return StartCoroutine(ExecuteMovement(CurrentMovement));
        }

        protected virtual IEnumerator ExecuteMovement(MovementType movementType)
        {
            yield return StartCoroutine(LaunchMovement(CurrentMovement));

            CurrentMovement = MovementType.None;
        }

        public override void Sleep()
        {
            base.Sleep();

            Aim?.Deactivate();
            Laser?.Deactivate();
            _castArea.enabled = false;
            _hearingPerimeter?.EraseSoundMark();
            _hearingPerimeter?.Deactivate();
        }

        public override void Wake()
        {
            base.Wake();

            _castArea.enabled = true;
            _hearingPerimeter?.Activate();

            if (_initState != StateType.Sleep)
            {
                Aim?.Activate();
            }
        }

        public override void Die(Transform killer = null, AudioFile sound = null, float volume = 1f)
        {
            if (Dead)
                return;

            if (killer != null)
            {
                _rigidbody?.AddForce((Transform.position - killer.position).normalized * 50, ForceMode2D.Impulse);
            }

            base.Die(killer, sound, volume);
        }

        public override void DoReset()
        {
            StopAllCoroutines();
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.angularVelocity = 0;
            Laser?.SetActive(false);
            base.DoReset();
        }


        protected float GetReactionFactor(StateType stateType)
        {
            switch (stateType)
            {
                case StateType.Sleep:
                    return 3;
                case StateType.Patrol:
                case StateType.Guard:
                    return 1.3f;
                case StateType.Wonder:
                    return .5f;
                default:
                    return 1;
            }
        }

        //protected override Action GetActionFromState(StateType stateType, StateType? nextState = null)
        //{
        //    switch (stateType)
        //    {
        //        case StateType.Sleep:
        //            return StartSleeping;

        //        case StateType.Wonder:
        //            return () => StartWondering(nextState.Value);

        //        case StateType.Check:
        //            return StartChecking;

        //        case StateType.Chase:
        //            return () => StartChasing(CurrentTarget);

        //        case StateType.Return:
        //            return StartReturning;

        //        case StateType.LookFor:
        //            return StartLookFor;

        //        case StateType.Guard:
        //        case StateType.Patrol:
        //            return StartGuarding;

        //        default:
        //            return null;
        //    }
        //}

        #region Events
        public void Hear(HearingArea hearingArea)
        {
            //TargetPosition = hearingArea.SourcePoint;

            //if (IsState(StateType.Chase) || (IsState(StateType.Wonder) && IsNextState(StateType.Check)))
            //    return;

            //Aim?.Activate();
            //if (!IsState(StateType.Check) && !IsState(StateType.Chase))
            //{
            //    SetState(StateType.Wonder, StateType.Check);
            //}
        }

        public void See(ISeeable seeable)
        {
            if (Seeing)
                return;

            // Visible when walking in the dark ?
            if (!seeable.Visible)
                return;

            if (CastUtils.LineCast(Transform.position, seeable.Transform.position, new int[] { Id, seeable.Id }))
                return;

            Seeing = true;

            // Temps de réaction
            if (TargetTransform == null)
            {
                _timeService.SlowDownImmediate();
                _timeService.SetTimeScaleProgressive(1);
                CurrentTarget = seeable;
            }

            //SetState(StateType.Chase);
        }

        #endregion
        //public abstract MovementType GetMovementType();

        //public abstract void MoveTo(Vector3 target);

        //public abstract void RotateTo(Vector3 target);

        //public abstract void LaunchMovement(MovementType movementType);

        #region Patrol
        public virtual void Patrol()
        {
            if (_coroutineService.IsCoroutineRunning(_movementId))
                return;

            if (_remainingTimeBeforeAction > 0)
            {
                _remainingTimeBeforeAction -= Time.deltaTime;
                return;
            }

            _remainingTimeBeforeAction = _delayBetweenActions;
            _movementId = _coroutineService.StartCoroutine(ExecuteNextMovement());
        }
        #endregion
    }
}