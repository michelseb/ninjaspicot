using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components
{
    public class LegTarget : Dynamic
    {
        [SerializeField] private RobotLeg _robotLeg;

        private float xOrigin;
        private float _currentSpeed;

        private void Start()
        {
            xOrigin = Transform.localPosition.x;
        }
        private void Update()
        {
            UpdateX();
            CheckGround();
        }

        private void UpdateX()
        {
            if (_currentSpeed != _robotLeg.Speed)
            {
                var pos = Transform.localPosition;
                _currentSpeed = _robotLeg.Speed;
                Transform.localPosition = new Vector3(xOrigin + _currentSpeed / 5, pos.y, pos.z);
            }
        }

        public void CheckGround()
        {
            // Check down
            var hit = CastUtils.RayCast(Transform.position, Vector3.down, .5f, layerMask: CastUtils.OBSTACLES);

            if (hit.collider != null)
            {
                Transform.position = hit.point + Vector2.up * .02f;
                return;
            }

            // Check up
            hit = CastUtils.RayCast(Transform.position, Vector3.up, 1, layerMask: CastUtils.OBSTACLES);

            if (hit.collider != null)
            {
                Transform.position = hit.point + Vector2.up * .02f;
            }
        }
    }
}