using UnityEngine;

public enum JumpMode
{
    Classic,
    Charge
}

public class Jumper : MonoBehaviour
{
    [SerializeField] private int _maxJumps;
    public bool Active { get; set; }
    public TrajectoryBase Trajectory { get; protected set; }
    public Vector3 TrajectoryOrigin { get; set; }
    public Vector3 TrajectoryDestination { get; set; }
    public Vector2 ChargeDestination { get; set; }
    protected float _strength;
    protected int _jumps;
    public JumpMode JumpMode;

    protected IDynamic _dynamicEntity;
    protected Stickiness _stickiness;
    protected PoolManager _poolManager;
    protected Transform _transform;
    protected virtual void Awake()
    {
        _dynamicEntity = GetComponent<IDynamic>();
        _stickiness = GetComponent<Stickiness>();
        _poolManager = PoolManager.Instance;
        _transform = transform;
    }

    protected virtual void Start()
    {
        _strength = 100;
        _jumps = _maxJumps;
        SetMaxJumps(_maxJumps);
        GainAllJumps();
    }

    public virtual void Jump(Vector2 direction)
    {
        _stickiness.Detach();

        direction = direction.normalized;

        LoseJump();
        _dynamicEntity.Rigidbody.velocity = Vector2.zero;
        _dynamicEntity.Rigidbody.AddForce(direction * _strength, ForceMode2D.Impulse);

        _poolManager.GetPoolable<Dash>(_transform.position, Quaternion.LookRotation(Vector3.forward, direction));

        if (TrajectoryInUse())
        {
            CommitJump();
        }
    }

    public virtual void Charge(Vector2 direction)
    {
        var initialPos = _dynamicEntity.Rigidbody.position;
        var pos = initialPos;
        var dir = (ChargeDestination - pos).normalized;
        pos += dir;

        while (Vector3.Dot(pos - initialPos, ChargeDestination - pos) > 0)
        {
            _poolManager.GetPoolable<HeroGhost>(pos, _dynamicEntity.Transform.rotation);
            pos += dir * 10;
        }

        _dynamicEntity.Rigidbody.position = ChargeDestination;
        Jump(direction);
    }

    public virtual void CommitJump()
    {
        if (Trajectory == null || !Trajectory.Active)
            return;

        if (Trajectory is ChargeTrajectory)
        {
            var charge = Trajectory as ChargeTrajectory;
            if (charge.Target != null)
            {
                charge.Target.Die(_transform);
            }
        }

        Trajectory.StartFading();
    }

    public virtual void CancelJump()
    {
        if (Trajectory == null || !Trajectory.Active)
            return;

        Trajectory.StartFading();
    }

    protected TrajectoryBase GetTrajectory<T>() where T : TrajectoryBase
    {
        if (Trajectory == null || !Trajectory.Active)
            return _poolManager.GetPoolable<T>(transform.position, Quaternion.identity);

        Trajectory.ReUse(_transform.position);
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
        return GetJumps() > 0;
    }

    public virtual bool ReadyToJump()
    {
        return CanJump() && TrajectoryInUse();
    }

    public T SetTrajectory<T>() where T : TrajectoryBase
    {
        if (TrajectoryInUse())
            return (T)Trajectory;

        Trajectory = GetTrajectory<T>();
        Trajectory.Strength = _strength;
        return (T)Trajectory;
    }

    public bool TrajectoryInUse()
    {
        return Trajectory != null && Trajectory.Used;
    }
}
