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
    Classic = 0,
    Charge = 1,
    Direct = 2
}

public enum JumpAction
{
    None = 0,
    Jump = 1,
    Dash = 2,
    LetGo = 3
}

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components
{
    public abstract class JumpSkill<T> : SkillBase, IAudio where T : Trajectory, new()
    {
        protected float _dashStrength;
        protected float _jumpStrength;
        protected float _jumpHeight;
        protected float _jumpMaxHeight;
        protected float _heightAtJumpInit;
        protected int _maxJumps;
        private Coroutine _slowDownMidAir;

        public T Trajectory { get; protected set; }
        public Vector3 TrajectoryOrigin { get; set; }
        public Vector3 TrajectoryDestination { get; set; }
        public Vector3 AimTarget { get; set; }
        public JumpAction CurrentJumpAction { get; protected set; }
        public virtual bool Ready => CanJump() && TrajectoryInUse;
        public virtual bool TrajectoryInUse => Trajectory != null && Trajectory.Used;

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
        protected Vector3[] _positions;
        protected bool _jumpPressing;

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
            _maxJumps = 2;
            _jumpStrength = 26;
            _jumpHeight = .5f;
            _jumpMaxHeight = 2.5f;
            _dashStrength = 30;

            _jumps = _maxJumps;
            SetMaxJumps(_maxJumps);
            GainAllJumps();

            _impactSound = _audioService.FindByName("Impact");

            Active = true;
        }

        private void FixedUpdate()
        {
            //Slow down at jump peak
            if (CurrentJumpAction == JumpAction.Jump && 
                _slowDownMidAir == null && 
                Rigidbody.velocity.y > 0 && 
                ((Rigidbody.position.y >= _jumpHeight + _heightAtJumpInit && !_jumpPressing) ||
                Rigidbody.position.y > _jumpMaxHeight + _heightAtJumpInit))
            {
                _slowDownMidAir = StartCoroutine(SlowDownMidAir());
            }
        }

        public void SetJumpPressing(bool pressing)
        {
            _jumpPressing = pressing;
        }

        public virtual void InitJump(Vector3 direction, Vector3 normalVector)
        {
            if (!CanJump())
                return;

            LoseJump();
            Rigidbody.velocity = Vector2.zero;
            _heightAtJumpInit = Rigidbody.position.y;

            var forceToApply = GetJumpForce(direction, normalVector, _jumpStrength);
            Rigidbody.AddForce(forceToApply, ForceMode2D.Impulse);

            CurrentJumpAction = JumpAction.Jump;
        }

        private Vector3 GetJumpForce(Vector3 direction, Vector3 normalVector, float strength)
        {
            // If under a roof, we just fall
            if (normalVector.y < -.1f)
                return normalVector;

            Vector3 directionVector;

            if (normalVector.y < .1f)
            {
                directionVector = normalVector + direction;
                return directionVector.normalized * strength / 3;
            }

            directionVector = Vector3.up * 2 + direction;
            return directionVector.normalized * strength;
        }

        public virtual void CalculatedJump(Vector2 velocity)
        {
            LoseJump();

            PoolHelper.Pool<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, velocity));

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

            //_cameraService.MainCamera.Shake(.3f, .1f);

            direction = direction.normalized;

            LoseJump();

            //StartCoroutine(DoNormalJump2(-direction * _strength));

            //Rigidbody.velocity = -direction * _strength;
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.AddForce(-direction * _jumpStrength, ForceMode2D.Impulse);

            PoolHelper.Pool<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, direction));

            if (TrajectoryInUse)
            {
                CommitJump();
            }
            CurrentJumpAction = JumpAction.Dash;
            Trajectory = null;
        }

        private IEnumerator DoNormalJump2(Vector2 trajectory)
        {
            Rigidbody.gravityScale = 0;
            Rigidbody.isKinematic = false;

            var oldPosition = Rigidbody.position;

            Rigidbody.velocity = trajectory;

            while (!Rigidbody.isKinematic)
            {
                while (Rigidbody.velocity.y > 0)
                    yield return null;
            }

            yield return null;
        }

        private IEnumerator DoNormalJump()
        {
            Rigidbody.gravityScale = 0;
            Rigidbody.isKinematic = false;
            int index = 1;
            var currentTarget = GetTrajectoryPositionAtIndex(index);
            var oldPosition = Rigidbody.position;
            var velocity = Vector3.zero;
            bool slowMoeing = false;
            float slowMoTime = .3f;
            var slowFactor = .00001f;

            while (currentTarget != Vector3.zero)
            {
                if (slowMoTime > 0 && Mathf.Abs(currentTarget.y - oldPosition.y) < .1f)
                {
                    slowMoeing = true;
                    slowFactor = .3f;
                }

                while (Vector3.Distance(Rigidbody.position, currentTarget) > .1f)
                {
                    slowMoTime -= Time.fixedDeltaTime;
                    //slowMoeing = slowMoeing && slowMoTime > 0;
                    if (slowMoeing && slowFactor > 0)
                    {
                        slowFactor -= Time.fixedDeltaTime;
                    }
                    if (slowFactor < 0)
                    {
                        slowFactor = 0;
                    }

                    Rigidbody.position = Vector3.SmoothDamp(Rigidbody.position, currentTarget, ref velocity, slowFactor);
                    yield return new WaitForFixedUpdate();
                }

                index++;
                oldPosition = currentTarget;
                currentTarget = GetTrajectoryPositionAtIndex(index);
            }

            Rigidbody.gravityScale = 1;
            Rigidbody.velocity = velocity;
        }

        public void RestoreGravity()
        {
            if (_slowDownMidAir != null)
            {
                StopCoroutine(_slowDownMidAir);
                _slowDownMidAir = null;
            }

            Rigidbody.gravityScale = 1;
        }

        private IEnumerator SlowDownMidAir()
        {
            var t = 0f;
            var originalVelocity = Rigidbody.velocity;

            while (t < 1 && originalVelocity.y > 0)
            {
                var factor = Mathf.Lerp(1, .5f, t * 8);//-Mathf.Abs(1 - t) + 1);
                Rigidbody.gravityScale = factor;
                Rigidbody.velocity = originalVelocity * factor;/*new Vector2(Rigidbody.velocity.x, originalVelocity.y * factor);*/
                originalVelocity += Physics2D.gravity * Time.fixedDeltaTime;
                t += Time.fixedDeltaTime * 3;

                yield return new WaitForFixedUpdate();
            }

            Rigidbody.gravityScale = 1;
            //Rigidbody.velocity = originalVelocity;
            _slowDownMidAir = null;
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
            Rigidbody.AddForce(direction * _jumpStrength * 3, ForceMode2D.Impulse);
            Rigidbody.gravityScale = 0;

            PoolHelper.Pool<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, direction));

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

            _audioService.PlaySound(this, JumpSound, _soundIntensity);

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

        private Vector3 GetTrajectoryPositionAtIndex(int index)
        {
            if (index == 1)
            {
                if (BaseUtils.IsNull(Trajectory))
                    return default;

                _positions = Trajectory.GetPositions();
            }

            if (index >= _positions.Length)
                return default;

            return _positions[index];
        }

        protected T GetTrajectory()
        {
            if (TrajectoryInUse && !(Trajectory is T))
            {
                Trajectory.StartFading();
            }

            if (Trajectory == null || !Trajectory.Active || !(Trajectory is T))
                return PoolHelper.Pool<T>(transform.position, Quaternion.identity);

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

            return !CastUtils.BoxCast(transform.position, Vector2.one * .1f, 0f, TrajectoryOrigin - TrajectoryDestination, 15f, Id,
            layer: CastUtils.GetMask("Obstacle"));
        }

        public T SetTrajectory()
        {
            if (TrajectoryInUse && Trajectory is T trajectory)
                return trajectory;

            Trajectory = GetTrajectory();
            Trajectory.Strength = _dashStrength;

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
            CurrentJumpAction = JumpAction.None;
            GainAllJumps();
        }
    }
}