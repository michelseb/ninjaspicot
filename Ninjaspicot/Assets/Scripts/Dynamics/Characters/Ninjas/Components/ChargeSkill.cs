using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components;
using ZepLink.RiceNinja.Dynamics.Effects;
using ZepLink.RiceNinja.Dynamics.Effects.Sounds;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components
{
    public class ChargeSkill : JumpSkill<ChargeTrajectory>
    {
        protected override string _soundName => "Dash";
        protected override float _soundIntensity => 1f;

        public Vector2 ChargeDestination => Trajectory?.GetLastPosition() ?? Transform.position;

        public void Bounce(Vector3 targetPosition)
        {
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.AddForce(((Transform.position - targetPosition).normalized + Vector3.up * 2) * 15, ForceMode2D.Impulse);
        }

        public override void CommitJump()
        {
            if (Trajectory.Collides && (Trajectory.Focusable == null || !Trajectory.Focusable.IsSilent))
            {
                _audioService.PlaySound(AudioSource, _impactSound);
                PoolHelper.PoolAt<SoundEffect>(ChargeDestination, Quaternion.identity, 5);
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
            {
                base.CommitJump();
                return;
            }

            //Bounce
            Bounce(Trajectory.Target.Transform.position);

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

            var initialPos = Rigidbody.position;
            var pos = initialPos;
            var dir = (ChargeDestination - pos).normalized;
            pos += dir;

            while (Vector3.Dot(pos - initialPos, ChargeDestination - pos) > 0)
            {
                var ghostSize = Mathf.Max((ChargeDestination - pos).magnitude, 1);
                PoolHelper.PoolAt<Ghost>(pos, Quaternion.AngleAxis(BaseUtils.GetAngleFromVector(dir) - 90, transform.forward), ghostSize);
                pos += dir / 2;
            }

            direction = direction.normalized;
            Rigidbody.position = ChargeDestination;

            NormalJump(direction);
            //_cameraService.MainCamera.Shake(.3f, .5f);
        }
    }
}