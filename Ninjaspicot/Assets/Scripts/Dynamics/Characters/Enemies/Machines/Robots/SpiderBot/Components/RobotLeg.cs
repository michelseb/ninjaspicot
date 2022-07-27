using System.Collections;
using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components
{
    public class RobotLeg : Dynamic
    {
        [SerializeField] private LegTarget _legTarget;
        [SerializeField] private int _index;

        private Vector2 _currentTarget;
        private const float REPLACEMENT_DISTANCE_THRESHOLD = 2f;
        private Coroutine _moveLeg;
        private SpiderBot _spiderBot;
        public float Speed => _spiderBot.MoveSpeed;
        public bool Grounded => _moveLeg == null;

        private void Awake()
        {
            _spiderBot = GetComponentInParent<SpiderBot>();
        }

        private void Start()
        {
            _legTarget.CheckGround();
            InitTarget();
        }
        private void Update()
        {
            UpdateTarget();
        }

        private void UpdateTarget()
        {
            if (_moveLeg != null || Vector3.Distance(_currentTarget, _legTarget.Transform.position) < REPLACEMENT_DISTANCE_THRESHOLD || _index != _spiderBot.MovingLegsIndex)
                return;

            LaunchMove();
        }

        public void InitTarget()
        {
            //_legTarget.Transform.position += Vector3.right * (_liftTiming /*+ _spiderBot.LegsSpeed / 100*/);
            _currentTarget = _legTarget.Transform.position;
            //Transform.position = _currentTarget;
        }

        public void FlipTarget()
        {
            var targetPos = _legTarget.Transform.position;
            var legPos = Transform.position;
            _legTarget.Transform.position = -targetPos + 2 * legPos;
        }

        public void LaunchMove()
        {
            if (_moveLeg != null) StopCoroutine(_moveLeg);
            _moveLeg = StartCoroutine(MoveLeg());
        }

        private IEnumerator MoveLeg()
        {
            var upPos = _currentTarget + Vector2.up * 2;

            while (Mathf.Abs(upPos.y - Transform.position.y) > .5f)
            {
                Transform.position = Vector3.MoveTowards(Transform.position, upPos, Time.deltaTime * _spiderBot.LegsSpeed);
                yield return null;
            }

            _currentTarget = _legTarget.Transform.position; //+ _spiderBot.LegsStabilizationFactor * Transform.right * Mathf.Sign(_spiderBot.Speed);

            while (Vector2.Distance(_currentTarget, Transform.position) > .5f)
            {
                Transform.position = Vector3.MoveTowards(Transform.position, _currentTarget, Time.deltaTime * _spiderBot.LegsSpeed);
                yield return null;
            }

            //Transform.position = _currentTarget;
            _moveLeg = null;

            yield return new WaitForSeconds(Speed / 100);

            _spiderBot.ChangeMovingLegs(1 - _index);
        }
    }
}