using UnityEngine;

public class Jumper : MonoBehaviour
{
    [SerializeField] private int _maxJumps;
    public bool Active { get; set; }
    protected float _strength;
    protected int _jumps;

    protected IDynamic _dynamicEntity;
    protected Stickiness _stickiness;
    protected PoolManager _poolManager;
    protected Trajectory _trajectory;

    protected virtual void Awake()
    {
        _dynamicEntity = GetComponent<IDynamic>();
        _stickiness = GetComponent<Stickiness>();
        _poolManager = PoolManager.Instance;
    }

    protected virtual void Start()
    {
        _strength = 100;
        _jumps = _maxJumps;
        SetMaxJumps(_maxJumps);
        GainAllJumps();
    }

    public virtual void Jump(Vector2 origin, Vector2 drag, float strength)
    {
        _stickiness.Detach();

        Vector2 forceToApply = origin - drag;
        LoseJump();
        _dynamicEntity.Rigidbody.velocity = new Vector2(0, 0);

        _dynamicEntity.Rigidbody.AddForce(forceToApply.normalized * strength, ForceMode2D.Impulse);

        _poolManager.GetPoolable<Dash>(transform.position, Quaternion.LookRotation(Vector3.forward, drag - origin));

        if (_trajectory.Used)
        {
            ReinitJump();
        }
    }

    public void ReinitJump()
    {
        if (_trajectory == null || !_trajectory.Active)
            return;

        _trajectory.StartFading();
    }

    protected Trajectory GetTrajectory()
    {
        if (_trajectory == null || !_trajectory.Active)
            return _poolManager.GetPoolable<Trajectory>(transform.position, Quaternion.identity);

        _trajectory.ReUse(transform.position);
        return _trajectory;
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
        return CanJump() && _trajectory != null;
    }
}
