using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components
{
    public class RobotLeg : Dynamic
    {
        [SerializeField] private LegTarget _legTarget;
        [SerializeField] private int _index;

        //private Vector2 _currentTarget;
        //private const float REPLACEMENT_DISTANCE_THRESHOLD = .2f;
        private SpiderBot _spiderBot;
        public int Index => _index;
        public float Speed => _spiderBot.MoveSpeed;
        public bool Grounded { get; private set; }

        private void Awake()
        {
            _spiderBot = GetComponentInParent<SpiderBot>();
        }

        private void Start()
        {
            Grounded = true;
            _legTarget.CheckGround();
            //InitTarget();
        }

        //private void Update()
        //{
        //    UpdateTarget();
        //}

        //private void UpdateTarget()
        //{
        //    if (_moveLeg != null || Vector3.Distance(_currentTarget, _legTarget.Transform.position) < REPLACEMENT_DISTANCE_THRESHOLD || _index != _spiderBot.MovingLegsIndex)
        //        return;

        //    LaunchMove();
        //}

        //public void InitTarget()
        //{
        //    //_legTarget.Transform.position += Vector3.right * (_liftTiming /*+ _spiderBot.LegsSpeed / 100*/);
        //    _currentTarget = _legTarget.Transform.position;
        //    //Transform.position = _currentTarget;
        //}

        public IEnumerator LaunchMove()
        {
            if (!Grounded)
                yield break;

            Grounded = false;

            yield return StartCoroutine(MoveLeg());
        }

        private IEnumerator MoveLeg()
        {
            var target = _legTarget.Transform;
            var upPos = target.position + Transform.up * .2f;

            while (Mathf.Abs(upPos.y - Transform.position.y) > .01f)
            {
                upPos = target.position + Transform.up * .2f;
                Transform.position = Vector3.MoveTowards(Transform.position, upPos, Time.deltaTime * _spiderBot.LegsSpeed);
                yield return null;
            }

            while (Mathf.Abs(target.position.y - Transform.position.y) > .01f)
            {
                Transform.position = Vector3.MoveTowards(Transform.position, target.position, Time.deltaTime * _spiderBot.LegsSpeed);
                yield return null;
            }

            Grounded = true;
        }
    }
}