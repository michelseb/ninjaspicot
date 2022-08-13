using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Skills;
using ZepLink.RiceNinja.Dynamics.Effects;
using ZepLink.RiceNinja.Dynamics.Scenery.Obstacles;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

public enum JumpMode
{
    Classic,
    Charge,
    Direct
}

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components
{
    public abstract class JumpSkill<T> : SkillBase, IAudio where T : Trajectory, new()
    {
        protected float _strength;
        protected int _maxJumps;

        public T Trajectory { get; protected set; }
        public Vector3 TrajectoryOrigin { get; set; }
        public Vector3 TrajectoryDestination { get; set; }
        public Vector2 ChargeDestination { get; set; }
        public Vector3 AimTarget { get; set; }
        public virtual bool Ready => CanJump() && TrajectoryInUse;
        public virtual bool TrajectoryInUse => Trajectory != null && Trajectory.Used;
        public override Transform Parent => Owner.Transform;

        protected abstract string _soundName { get; }
        protected abstract float _soundIntensity { get; }
        private AudioFile _sound;
        public AudioFile JumpSound { get { if (BaseUtils.IsNull(_sound)) _sound = _audioService.FindByName(_soundName); return _sound; } }

        private AudioSource _audioSource;
        public AudioSource AudioSource { get { if (_audioSource == null) _audioSource = GetComponent<AudioSource>(); return _audioSource; } }

        protected int _jumps;
        protected AudioFile _impactSound;
        protected ICameraService _cameraService;
        protected IAudioService _audioService;
        protected ITimeService _timeService;

        protected Coroutine _directJump;

        protected virtual void Awake()
        {
            _cameraService = ServiceFinder.Get<ICameraService>();
            _timeService = ServiceFinder.Get<ITimeService>();
            _audioService = ServiceFinder.Get<IAudioService>();

            _audioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            // TODO : make this configurable
            _maxJumps = 1;
            _strength = 100;

            _jumps = _maxJumps;
            SetMaxJumps(_maxJumps);
            GainAllJumps();
           
            _impactSound = _audioService.FindByName("Impact");

            Active = true;
        }

        public virtual void CalculatedJump(Vector2 velocity)
        {
            LoseJump();

            PoolHelper.PoolAt<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, velocity));

            if (TrajectoryInUse)
            {
                CommitJump();
            }
        }

        public virtual void NormalJump(Vector2 direction)
        {
            if (GetJumps() <= 0)
            {
                _timeService.SetNormalTime();
            }

            _cameraService.MainCamera.Shake(.3f, .1f);

            direction = direction.normalized;

            LoseJump();

            Rigidbody.velocity = Vector2.zero;
            Rigidbody.AddForce(direction * _strength, ForceMode2D.Impulse);

            PoolHelper.PoolAt<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, direction));

            if (TrajectoryInUse)
            {
                CommitJump();
            }

            Trajectory = null;
        }


        public virtual void LaunchDirectJump(Vector3 target)
        {
            if (_directJump != null) StopCoroutine(_directJump);
            _directJump = StartCoroutine(DirectJump(target));
        }

        public virtual IEnumerator DirectJump(Vector3 target)
        {
            var direction = (target - Transform.position).normalized;

            LoseJump();

            Rigidbody.velocity = Vector2.zero;
            Rigidbody.AddForce(direction * _strength * 3, ForceMode2D.Impulse);
            Rigidbody.gravityScale = 0;

            PoolHelper.PoolAt<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, direction));

            if (TrajectoryInUse)
            {
                CommitJump();
            }

            while (Vector3.Dot(direction, target - Transform.position) > 0.5f)
            {
                yield return null;
            }

            _directJump = null;
        }

        public virtual void CommitJump()
        {
            if (Trajectory == null || !Trajectory.Active)
                return;

            _audioService.PlaySound(this, _sound, _soundIntensity);

            Trajectory.StartFading();
        }

        public virtual void CancelJump()
        {
            if (Trajectory == null || !Trajectory.Active)
                return;

            Trajectory.StartFading();
            _timeService.SetNormalTime();
        }

        public void SetJumpPositions(Vector3 origin, Vector3 destination)
        {
            TrajectoryOrigin = origin;
            TrajectoryDestination = destination;
        }

        protected T GetTrajectory()
        {
            if (TrajectoryInUse && !(Trajectory is T))
            {
                Trajectory.StartFading();
            }

            if (Trajectory == null || !Trajectory.Active || !(Trajectory is T))
                return PoolHelper.PoolAt<T>(transform.position, Quaternion.identity);

            Trajectory.ReUse(Transform.position);
            return Trajectory;
        }

        public void LoseJump()
        {
            _jumps--;
        }

        public void LoseAllJumps()
        {
            _jumps = 0;
        }

        public int GetJumps()
        {
            return _jumps;
        }

        public int GetMaxJumps()
        {
            return _maxJumps;
        }

        public void SetMaxJumps(int amount)
        {
            _maxJumps = amount;
        }

        public void SetJumps(int amount)
        {
            _jumps = amount;
        }

        public void GainJumps(int amount)
        {
            _jumps += amount;

            if (_jumps > _maxJumps)
            {
                GainAllJumps();
            }
        }

        public void GainAllJumps()
        {
            _jumps = _maxJumps;
        }

        public virtual bool CanJump()
        {
            if (!Active || GetJumps() <= 0)
                return false;

            return !CastUtils.BoxCast(transform.position, Vector2.one, 0f, TrajectoryOrigin - TrajectoryDestination, 15f, Id,
            layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")));
        }

        public T SetTrajectory()
        {
            if (TrajectoryInUse && Trajectory is T trajectory)
                return trajectory;

            Trajectory = GetTrajectory();
            Trajectory.Strength = _strength;

            return Trajectory;
        }

        public abstract void Jump(Vector2 direction);

        public void DisplayTrajectory(Vector2 direction)
        {
            if (!CanJump())
            {
                if (TrajectoryInUse)
                {
                    CancelJump();
                }

                return;
            }

            SetTrajectory();

            Trajectory.DrawTrajectory(Transform.position, direction);
        }

        public override void LandOn(Obstacle obstacle, Vector3 contactPoint)
        {
            GainAllJumps();
        }
    }
}