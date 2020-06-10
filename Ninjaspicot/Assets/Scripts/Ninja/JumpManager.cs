using UnityEngine;

public class JumpManager : MonoBehaviour
{
    [SerializeField] private int _maxJumps;
    public bool Active { get; set; }

    private float _strength;
    private int _jumps;

    private IDynamic _dynamicEntity;
    private DynamicInteraction _dynamicInteraction;
    private Stickiness _stickiness;
    private TimeManager _timeManager;
    private CameraBehaviour _cameraBehaviour;
    private TouchManager _touchManager;
    private PoolManager _poolManager;
    private Trajectory _trajectory;

    private void Awake()
    {
        _dynamicEntity = GetComponent<IDynamic>();
        _stickiness = GetComponent<Stickiness>();
        _dynamicInteraction = GetComponent<DynamicInteraction>();
        _poolManager = PoolManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _timeManager = TimeManager.Instance;
        _touchManager = TouchManager.Instance;
    }

    private void Start()
    {
        _strength = 100;
        _jumps = _maxJumps;
        SetMaxJumps(_maxJumps);
        GainAllJumps();
    }

    private void Update()
    {
        if (!Active)
            return;

        if (_touchManager.Touching)
        {
            if (CanJump())
            {
                _trajectory = GetTrajectory();
                _trajectory.DrawTrajectory(transform.position, _touchManager.TouchDrag, _touchManager.RawTouchOrigin, _strength);
            }
            else if (_trajectory != null)
            {
                ReinitJump();
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (ReadyToJump())
            {
                if (_trajectory != null && !_trajectory.IsClear(transform.position, 2))//Add ninja to new layer
                {
                    _trajectory.ReinitTrajectory();
                    _timeManager.SetNormalTime();
                }
                else
                {
                    _stickiness.StopWalking();
                    Jump(_touchManager.RawTouchOrigin, _touchManager.TouchDrag, _strength);
                }
                _touchManager.ReinitDrag();
            }
        }
    }

    public void Jump(Vector2 origin, Vector2 drag, float strength)
    {
        if (_dynamicInteraction.Interacting)
        {
            _dynamicInteraction.StopInteraction(true);
        }

        _stickiness.Detach();

        Vector2 forceToApply = origin - drag;
        LoseJump();
        _dynamicEntity.Rigidbody.velocity = new Vector2(0, 0);

        if (GetJumps() <= 0)
        {
            _timeManager.SetNormalTime();
        }

        _dynamicEntity.Rigidbody.AddForce(forceToApply.normalized * strength, ForceMode2D.Impulse);

        _poolManager.GetPoolable<Dash>(transform.position, Quaternion.LookRotation(Vector3.forward, drag - origin));

        if (_trajectory != null)
        {
            ReinitJump();
        }

        _cameraBehaviour.DoShake(.3f, .1f);
    }

    public void ReinitJump()
    {
        if (_trajectory == null)
            return;

        _trajectory.ReinitTrajectory();
        _trajectory = null;
    }

    private Trajectory GetTrajectory()
    {
        if (_trajectory == null)
        {
            return _poolManager.GetPoolable<Trajectory>(transform.position, Quaternion.identity);
        }

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

    public bool CanJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.CanJump();

        var boxCast = Utils.BoxCast(transform.position, Vector2.one, 0f, _touchManager.RawTouchOrigin - _touchManager.TouchDrag, 5f, Hero.Instance.Id/*, display: true*/,
             layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));

        return GetJumps() > 0 && _touchManager.Dragging && !boxCast;
    }

    public bool ReadyToJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.ReadyToJump();

        return CanJump() && _trajectory != null;
    }
}
