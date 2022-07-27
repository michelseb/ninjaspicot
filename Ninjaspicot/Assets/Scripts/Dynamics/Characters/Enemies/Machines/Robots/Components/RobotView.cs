using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Viewing;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Robots.Components
{
    public class RobotView : FieldOfView
    {
        private GuardRobotBall _robot;

        protected override void Awake()
        {
            base.Awake();
            _robot = GetComponentInParent<GuardRobotBall>();
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnTriggerEnter2D(collider);

            if (!collider.TryGetComponent(out ISeeable seeable))
                return;

            _robot.See(seeable);
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            base.OnTriggerExit2D(collider);

            if (collider.CompareTag("hero"))
            {
                _robot.Seeing = false;
            }
        }
    }
}