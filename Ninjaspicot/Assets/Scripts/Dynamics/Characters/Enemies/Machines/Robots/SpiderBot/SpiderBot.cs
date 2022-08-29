using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots
{
    public class SpiderBot : Robot
    {
        [SerializeField] private Transform _body;
        [SerializeField] private float _stepDuration = .3f;
        [SerializeField] private float _stepDistance = .5f;

        public int MovingLegsIndex { get; private set; }

        private Vector3 _bodyOffset;

        // Caching legs to recover body rotation
        private Transform _legIndex0;
        private Transform _legIndex1;

        public override SpriteRenderer Renderer
        {
            get
            {
                if (BaseUtils.IsNull(_renderer))
                {
                    _renderer = _head.GetComponentInChildren<SpriteRenderer>();
                }

                return _renderer;
            }
        }

        protected Coroutine _lookAt;
        private List<RobotLeg> _robotLegs;

        protected override void Awake()
        {
            base.Awake();

            _robotLegs = GetComponentsInChildren<RobotLeg>().ToList();
        }

        protected override void Start()
        {
            base.Start();

            _robotLegs.ForEach(x => x.SetBody(_body));

            _bodyOffset = _body.position - GetLegMeanPosition();

            _legIndex0 = _robotLegs.FirstOrDefault(l => l.Index == 0).Transform;
            _legIndex1 = _robotLegs.FirstOrDefault(l => l.Index == 1).Transform;
        }

        protected override void Update()
        {
            base.Update();

            var meanPosition = GetLegMeanPosition();
            _body.position = meanPosition + _bodyOffset;
            _body.rotation = Quaternion.Euler(0, 0, 90) * GetBodyRotation();
        }

        private Vector3 GetLegMeanPosition()
        {
            return _robotLegs.Select(l => l.Transform.position).Aggregate((sum, vector) => sum + vector) / _robotLegs.Count;
        }

        private Quaternion GetBodyRotation()
        {
            return Quaternion.LookRotation(Vector3.forward, _legIndex0.position - _legIndex1.position);
        }

        private bool IsGapAhead()
        {
            var cast = CastUtils.RayCast(_head.position, new Vector2(MoveDirection, -Transform.up.y), 1.5f, layerMask: CastUtils.OBSTACLES).collider == null;
            Debug.Log("Gap ahead : " + cast);

            return cast;
        }

        private bool IsWallAhead()
        {
            var cast = CastUtils.RayCast(_head.position, Vector2.right * MoveDirection, 1f, layerMask: CastUtils.OBSTACLES).collider != null;
            Debug.Log("Wall ahead : " + cast);

            return cast;
        }

        //#region Check
        //protected override void Check()
        //{
        //}
        //#endregion

        //#region Chase
        //protected override void Chase(Vector3 target)
        //{
        //}
        //#endregion

        //#region LookFor

        //protected override void LookFor()
        //{
        //    if (_lookAt == null)
        //    {
        //        _lookAt = StartCoroutine(LookAtRandom());
        //    }
        //    Guard();
        //}

        //protected virtual IEnumerator LookAtRandom()
        //{
        //    float elapsedTime = 0;
        //    float delay = 2;
        //    var direction = Random.insideUnitCircle.normalized;

        //    while (elapsedTime < delay)
        //    {
        //        elapsedTime += Time.deltaTime;
        //        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _body.TransformDirection(direction)), RotateSpeed);
        //        yield return null;
        //    }

        //    _lookAt = null;
        //}
        //#endregion

        //#region Return
        //protected override void Return()
        //{
        //}
        //#endregion

        //#region Guard
        //protected override void Guard() { }
        //#endregion

        protected override MovementType GetPatrolMovementType()
        {
            return IsGapAhead() || IsWallAhead() ? MovementType.Rotate : MovementType.Move;
            //if ()
            //{
            //    Flip();
            //    return MovementType.Rotate;
            //}

            //if (_remainingTimeBeforeAction <= 0)
            //{
            //    _remainingTimeBeforeAction = DelayBetweenLegSwitch;
            //    return MovementType.Move;
            //}

            //return MovementType.None;
        }

        protected override IEnumerator MoveTo(Vector3 target)
        {
            SwitchMovingLegs();

            var movingLegs = _robotLegs.Where(l => l.Index == MovingLegsIndex).ToArray();

            var movePoint = _stepDistance * MoveDirection;

            var frontLegMove = _coroutineService.StartCoroutine(_robotLegs.FirstOrDefault(l => l.Index == MovingLegsIndex).LaunchMove(_stepDuration, movePoint));
            var backLegMove = _coroutineService.StartCoroutine(_robotLegs.LastOrDefault(l => l.Index == MovingLegsIndex).LaunchMove(_stepDuration, movePoint));

            while (_coroutineService.IsCoroutineRunning(frontLegMove) || _coroutineService.IsCoroutineRunning(backLegMove))
                yield return null;

        }

        protected override IEnumerator RotateTo(Vector3 target)
        {
            var t = 0f;
            var initRotation = _head.rotation;
            var targetRotation = Quaternion.Euler(0, 0, 90) * Quaternion.LookRotation(Vector3.forward, target);

            while (t < 1)
            {
                t += RotateSpeed;
                _head.rotation = Quaternion.Slerp(initRotation, targetRotation, t);

                yield return null;
            }
        }

        public override void Die(Transform killer = null, AudioFile sound = null, float volume = 1)
        {
            _hearingPerimeter.EraseSoundMark();
            base.Die(killer, sound, volume);
        }

        public override void DoReset()
        {
            Seeing = false;
            CurrentTarget = default;

            if (_lookAt != null)
            {
                StopCoroutine(_lookAt);
                _lookAt = null;
            }

            base.DoReset();
        }

        private void SwitchMovingLegs()
        {
            MovingLegsIndex = 1 - MovingLegsIndex;
        }

        //public override void MoveTo(Vector3 target)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void RotateTo(Vector3 target)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public override void LaunchMovement(MovementType movementType)
        //{
        //    throw new System.NotImplementedException();
        //}

        protected override IEnumerator LaunchMovement(MovementType movementType)
        {
            switch (CurrentMovement)
            {
                case MovementType.Rotate:
                    Flip();
                    yield return StartCoroutine(RotateTo(Vector2.right * MoveDirection));
                    break;

                case MovementType.Move:
                    yield return StartCoroutine(MoveTo(default));
                    break;
            }
        }
    }
}