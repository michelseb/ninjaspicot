using System.Collections;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components
{
    public class RobotLeg : Dynamic
    {
        [SerializeField] private int _index;

        private SpiderBot _spiderBot;
        public int Index => _index;
        public float Speed => _spiderBot.MoveSpeed;
        public bool Grounded { get; private set; }

        private Transform _body;
        private Vector3 _deltaPos;

        private void Awake()
        {
            _spiderBot = GetComponentInParent<SpiderBot>();
        }

        private void Start()
        {
            Grounded = true;
        }

        public void SetBody(Transform body)
        {
            _body = body;
            _deltaPos = Transform.position - body.position;
        }

        public IEnumerator LaunchMove(float duration, float moveVector)
        {
            if (!Grounded || _body == null)
                yield break;

            Grounded = false;

            yield return StartCoroutine(MoveLeg(duration, moveVector));
        }

        private IEnumerator MoveLeg(float duration, float moveVector)
        {
            var moveDistance = duration * moveVector;
            var checkPos = _body.position + _deltaPos + Vector3.right * moveDistance;

            var casts = new RaycastHit2D[]
            {
                CastUtils.RayCast(checkPos, -_body.up, .5f, layerMask: CastUtils.OBSTACLES),
                CastUtils.RayCast(checkPos, _body.up, 1, layerMask: CastUtils.OBSTACLES)
            };
            Debug.DrawRay(checkPos, -_body.up, Color.red, 1);

            var target = casts.FirstOrDefault(c => c).point;

            if (target == default)
            {
                Grounded = true;
                yield break;
            }

            var originBodyY = _body.localPosition.y;
            var initPos = Transform.position;
            var halfDuration = duration / 2;
            var halfDistance = moveDistance / 2;
            var meanHeight = (target.y + initPos.y) / 2;
            var targetPos = new Vector3(initPos.x + halfDistance, meanHeight + Transform.up.y * Mathf.Abs(halfDistance));

            yield return StartCoroutine(LegInterpolate(halfDuration, Transform.position, targetPos, originBodyY, originBodyY + Transform.up.y));
            yield return StartCoroutine(LegInterpolate(halfDuration, Transform.position, target, _body.localPosition.y, originBodyY));

            _body.localPosition = new Vector3(_body.localPosition.x, originBodyY);

            Grounded = true;
        }

        private IEnumerator LegInterpolate(float duration, Vector3 initPos, Vector3 targetPos, float initBodyY, float targetBodyY)
        {
            var t = 0f;

            while (t < duration)
            {
                Transform.position = Vector3.Lerp(initPos, targetPos, t / duration);

                var bodyTargetY = Mathf.Lerp(initBodyY, targetBodyY, t / duration);
                _body.localPosition = new Vector3(_body.localPosition.x, bodyTargetY);

                t += Time.deltaTime;

                yield return null;
            }
        }
    }
}