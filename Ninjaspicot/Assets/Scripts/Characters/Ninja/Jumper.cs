using System.Collections;
using UnityEngine;

public enum JumpMode
{
    Classic,
    Charge,
    Direct
}

public class Jumper : Dynamic
{
    [SerializeField] private int _maxJumps;
    [SerializeField] protected float _strength;

    public bool Active { get; set; }
    public TrajectoryBase Trajectory { get; protected set; }
    public Vector3 TrajectoryOrigin { get; set; }
    public Vector3 TrajectoryDestination { get; set; }
    public Vector2 ChargeDestination { get; set; }
    public Vector3 AimTarget { get; set; }
    public JumpMode JumpMode { get; set; }
    protected int _jumps;

    protected IDynamic _dynamicEntity;
    public IDynamic DynamicEntity => _dynamicEntity;
    protected Stickiness _stickiness;
    protected PoolManager _poolManager;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected Audio _normalJumpSound;
    protected Audio _chargeJumpSound;
    protected Audio _impactSound;
    protected DynamicInteraction _dynamicInteraction;
    protected TimeManager _timeManager;
    protected CameraBehaviour _cameraBehaviour;

    protected Coroutine _directJump;
    protected float _initGravity;
    protected virtual void Awake()
    {
        _dynamicEntity = GetComponent<IDynamic>();
        _dynamicInteraction = GetComponent<DynamicInteraction>();
        _cameraBehaviour = CameraBehaviour.Instance;
        _timeManager = TimeManager.Instance;
        _stickiness = GetComponent<Stickiness>();
        _poolManager = PoolManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _initGravity = _dynamicEntity.Rigidbody.gravityScale;
    }

    protected virtual void Start()
    {
        _jumps = _maxJumps;
        SetMaxJumps(_maxJumps);
        GainAllJumps();

        _audioManager = AudioManager.Instance;
        _normalJumpSound = _audioManager.FindAudioByName("Jump");
        _chargeJumpSound = _audioManager.FindAudioByName("Dash");
        _impactSound = _audioManager.FindAudioByName("Impact");

        Active = true;
    }

    public virtual void CalculatedJump(Vector2 velocity)
    {
        _stickiness.Detach();
        _stickiness.StopWalking(false);
        LoseJump();
        _dynamicEntity.Rigidbody.velocity = velocity;

        _poolManager.GetPoolable<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, velocity));

        if (TrajectoryInUse())
        {
            CommitJump();
        }
    }

    public virtual void NormalJump(Vector2 direction)
    {
        if (_dynamicInteraction.Interacting)
        {
            _dynamicInteraction.StopInteraction(true);
        }

        if (GetJumps() <= 0)
        {
            _timeManager.SetNormalTime();
        }
        _cameraBehaviour.DoShake(.3f, .1f);

        _stickiness.Detach();

        direction = direction.normalized;

        LoseJump();
        _dynamicEntity.Rigidbody.velocity = Vector2.zero;
        _dynamicEntity.Rigidbody.AddForce(direction * _strength, ForceMode2D.Impulse);

        _poolManager.GetPoolable<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, direction));

        if (TrajectoryInUse())
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

        _stickiness.Detach();
        LoseJump();

        _dynamicEntity.Rigidbody.velocity = Vector2.zero;
        _dynamicEntity.Rigidbody.AddForce(direction * _strength * 3, ForceMode2D.Impulse);
        _dynamicEntity.Rigidbody.gravityScale = 0;
        _poolManager.GetPoolable<Dash>(Transform.position, Quaternion.LookRotation(Vector3.forward, direction));

        if (TrajectoryInUse())
        {
            CommitJump();
        }

        while (Vector3.Dot(direction, target - Transform.position) > 0.5f)
        {
            yield return null;
        }

        _dynamicEntity.Rigidbody.gravityScale = _initGravity;
        _directJump = null;
    }

    public virtual void Charge(Vector2 direction)
    {
        var initialPos = _dynamicEntity.Rigidbody.position;
        var pos = initialPos;
        var dir = (ChargeDestination - pos).normalized;
        pos += dir;

        while (Vector3.Dot(pos - initialPos, ChargeDestination - pos) > 0)
        {
            _poolManager.GetPoolable<HeroGhost>(pos, Quaternion.AngleAxis(Utils.GetAngleFromVector(dir) - 90, transform.forward), Mathf.Max((ChargeDestination - pos).magnitude / 15, 1));
            pos += dir * 10;
        }

        direction = direction.normalized;
        _dynamicEntity.Rigidbody.position = ChargeDestination - (direction * 7);

        NormalJump(direction);
        _cameraBehaviour.DoShake(.3f, .5f);
    }

    public virtual void CommitJump()
    {
        if (Trajectory == null || !Trajectory.Active)
            return;

        if (Trajectory is ChargeTrajectory chargeTrajectory)
        {
            _audioManager.PlaySound(_audioSource, _chargeJumpSound, .3f);

            if (chargeTrajectory.Collides && (chargeTrajectory.Focusable == null || !chargeTrajectory.Focusable.IsSilent))
            {
                _audioManager.PlaySound(_audioSource, _impactSound);
                _poolManager.GetPoolable<SoundEffect>(ChargeDestination, Quaternion.identity, 5);
            }
        }
        else
        {
            _audioManager.PlaySound(_audioSource, _normalJumpSound);
        }

        Trajectory.StartFading();

        if (Trajectory is ChargeTrajectory c)
        {
            c.Bonuses.ForEach(x => x.Take());
            c.Interactives.ForEach(x => x.Activate());

            if (c.Target == null)
                return;

            //Bounce
            Bounce(c.Target.Transform.position);

            c.Target.Die(Transform);
            c.Target = null;
            GainJumps(1);
            _timeManager.SlowDown();
            _timeManager.StartTimeRestore();

        }
    }

    public virtual void CancelJump()
    {
        if (Trajectory == null || !Trajectory.Active)
            return;

        Trajectory.StartFading();
        _timeManager.SetNormalTime();
    }

    public void SetJumpPositions(Vector3 origin, Vector3 destination)
    {
        TrajectoryOrigin = origin;
        TrajectoryDestination = destination;
    }

    protected TrajectoryBase GetTrajectory<T>() where T : TrajectoryBase
    {
        if (TrajectoryInUse() && !(Trajectory is T))
        {
            Trajectory.StartFading();
        }

        if (Trajectory == null || !Trajectory.Active || !(Trajectory is T))
            return _poolManager.GetPoolable<T>(transform.position, Quaternion.identity);

        Trajectory.ReUse(Transform.position);
        return Trajectory;
    }

    public void Bounce(Vector3 targetPosition)
    {
        _stickiness.Rigidbody.velocity = Vector2.zero;
        _stickiness.Rigidbody.AddForce(((_stickiness.Transform.position - targetPosition).normalized + Vector3.up * 2) * 15, ForceMode2D.Impulse);
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
        if (CompareTag("Dynamic"))
            return Hero.Instance.Jumper.CanJump();

        if (!Active || GetJumps() <= 0)
            return false;

        return !Utils.BoxCast(transform.position, Vector2.one, 0f, TrajectoryOrigin - TrajectoryDestination, 15f, Hero.Instance.Id,
        layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")));
    }

    public virtual bool ReadyToJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.Jumper.ReadyToJump();

        return CanJump() && TrajectoryInUse();
    }

    public T SetTrajectory<T>() where T : TrajectoryBase
    {
        if (TrajectoryInUse() && Trajectory is T trajectory)
            return trajectory;

        Trajectory = GetTrajectory<T>();
        Trajectory.Strength = _strength;
        return (T)Trajectory;
    }

    public bool TrajectoryInUse()
    {
        return Trajectory != null && Trajectory.Used;
    }
}
