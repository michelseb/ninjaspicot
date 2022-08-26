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

        public int MovingLegsIndex { get; private set; }
        public float LegsSpeed => MoveSpeed == 0 ? LegsSpeed : Mathf.Abs(MoveSpeed) * 3;
        public float DelayBetweenLegSwitch => 1f / (2 * LegsSpeed + 1f);

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

            CurrentMovement = MovementType.Rotate;
            //StartCoroutine(ExecuteMovement(CurrentMovement));
        }


        private bool IsGapAhead()
        {
            return !CastUtils.RayCast(_body.position, new Vector2(2 * MoveDirection, -Transform.up.y), 1.5f, layerMask: CastUtils.OBSTACLES);
        }

        private bool IsWallAhead()
        {
            return CastUtils.RayCast(_body.position, Transform.right * MoveDirection, 1f, layerMask: CastUtils.OBSTACLES);
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

            var backLegName = $"backLegMove_{Id}"; 
            var frontLegName = $"frontLegMove_{Id}"; 

            var frontLegMove = _coroutineService.StartCoroutine(_robotLegs.FirstOrDefault(l => l.Index == MovingLegsIndex).LaunchMove(), key: backLegName);
            var backLegMove = _coroutineService.StartCoroutine(_robotLegs.LastOrDefault(l => l.Index == MovingLegsIndex).LaunchMove(), key: frontLegName);

            while (_coroutineService.IsCoroutineRunning(frontLegName) || _coroutineService.IsCoroutineRunning(backLegName))
            {
                _body.Translate(_body.right * MoveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        protected override IEnumerator RotateTo(Vector3 target)
        {
            var t = 0f;
            var initRotation = _head.rotation;
            var targetRotation = Quaternion.Euler(0, 0, 90) * Quaternion.LookRotation(Vector3.forward, target);

            while (t < 1)
            {
                t += Time.deltaTime * RotateSpeed;
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
            _remainingTimeBeforeAction = DelayBetweenLegSwitch;
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
                    yield return StartCoroutine(RotateTo(_body.right * MoveDirection));
                    break;

                case MovementType.Move:
                    yield return StartCoroutine(MoveTo(_body.right * MoveDirection));
                    break;
            }
        }
    }
}