using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Actions
{
    public abstract class TargetAction : MovementAction
    {
        protected Transform _target;

        public TargetAction(Transform target, float speed) : base(speed)
        {
            _target = target;
        }
    }
}