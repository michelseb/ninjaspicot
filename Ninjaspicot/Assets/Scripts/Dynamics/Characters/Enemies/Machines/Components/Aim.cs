using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Viewing;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Components
{
    public class Aim : FieldOfView
    {
        public bool TargetInRange { get; internal set; }
        public bool TargetInView { get; internal set; }

        //protected virtual void Update()
        //{
        //    if (!Active)
        //        return;

        //    var dist = Vector3.Distance(Viewer.CurrentTarget.Transform.position, Transform.position);
        //    TargetInRange = dist < _size;
        //    if (!TargetInRange && TargetInView)
        //    {
        //        TargetInView = false;
        //    }
        //}

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnTriggerEnter2D(collider);

            if (!collider.TryGetComponent(out ISeeable seeable))
                return;

            Viewer.See(seeable);
        }

        protected virtual void OnTriggerStay2D(Collider2D collider)
        {
            if (!collider.TryGetComponent(out ISeeable seeable))
                return;

            Viewer.See(seeable);
        }

        protected override void OnTriggerExit2D(Collider2D collider)
        {
            if (!collider.CompareTag("hero"))
                return;

            base.OnTriggerExit2D(collider);
            TargetInView = false;
        }

        public bool TargetCentered(Transform origin, string targetTag, int ignoreId = 0)
        {
            if (!TargetInView)
                return false;

            var collider = CastUtils.RayCast(origin.position, origin.up, ignore: ignoreId).collider;

            if (collider == null)
                return false;

            return collider.CompareTag(targetTag);
        }

        public bool TargetAimedAt(IKillable target, int ignoreId = 0)
        {
            if (target == null || !TargetInView)
                return false;

            var hit = CastUtils.LineCast(Transform.position, target.Transform.position, new int[] { ignoreId }, false, target.Transform.tag);

            return hit && hit.transform.CompareTag(target.Transform.tag);
        }

        public bool TargetVisible(IKillable target, int ignoreId = 0)
        {
            if (target == null || !TargetInRange)
                return false;

            var hit = CastUtils.LineCast(Transform.position, target.Transform.position, new int[] { ignoreId }, false, target.Transform.tag);

            return hit && hit.transform.CompareTag(target.Transform.tag);
        }
    }
}