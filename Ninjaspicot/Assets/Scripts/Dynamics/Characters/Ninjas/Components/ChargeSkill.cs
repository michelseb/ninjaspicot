using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components;
using ZepLink.RiceNinja.Dynamics.Effects;
using ZepLink.RiceNinja.Dynamics.Effects.Sounds;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components
{
    public class ChargeSkill : JumpSkill<ChargeTrajectory>
    {
        protected override string _soundName => "Dash";
        protected override float _soundIntensity => 1f;

        public virtual void Charge(Vector2 direction)
        {
            var initialPos = Rigidbody.position;
            var pos = initialPos;
            var dir = (ChargeDestination - pos).normalized;
            pos += dir;

            while (Vector3.Dot(pos - initialPos, ChargeDestination - pos) > 0)
            {
                _poolService.GetPoolable<Ghost>(pos, Quaternion.AngleAxis(BaseUtils.GetAngleFromVector(dir) - 90, transform.forward), Mathf.Max((ChargeDestination - pos).magnitude / 15, 1));
                pos += dir * 10;
            }

            direction = direction.normalized;
            Rigidbody.position = ChargeDestination - (direction * 7);

            NormalJump(direction);
            _cameraService.MainCamera.Shake(.3f, .5f);
        }

        //public void Bounce(Vector3 targetPosition)
        //{
        //    _stickiness.Rigidbody.velocity = Vector2.zero;
        //    _stickiness.Rigidbody.AddForce(((_stickiness.Transform.position - targetPosition).normalized + Vector3.up * 2) * 15, ForceMode2D.Impulse);
        //}

        public override void CommitJump()
        {
            if (Trajectory.Collides && (Trajectory.Focusable == null || !Trajectory.Focusable.IsSilent))
            {
                _audioService.PlaySound(AudioSource, _impactSound);
                _poolService.GetPoolable<SoundEffect>(ChargeDestination, Quaternion.identity, 5);
            }

            if (Owner is IPicker picker)
            {
                Trajectory.Bonuses.ForEach(x => x.TakeBy(picker));
            }

            if (Owner is IActivator activator)
            {
                Trajectory.Interactives.ForEach(x => x.Activate(activator));
            }

            if (Trajectory.Target == null)
                return;

            //Bounce
            //Bounce(Trajectory.Target.Transform.position);

            Trajectory.Target.Die(Transform);
            Trajectory.Target = null;
            GainJumps(1);
            _timeService.SlowDownImmediate();
            _timeService.SetTimeScaleProgressive(1, 2);

            base.CommitJump();
        }

        public override void Jump(Vector2 direction)
        {
            if (!Ready)
                return;
        }
    }
}