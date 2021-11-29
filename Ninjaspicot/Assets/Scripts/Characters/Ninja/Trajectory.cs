using UnityEngine;

public class Trajectory : Dynamic, IPoolable
{
    public bool Used { get; protected set; }
    public bool Active { get; protected set; }

    public CustomColor Color { get; set; }
    public IFocusable Target { get; set; }

    public bool Aiming => _aimIndicator != null;

    protected LineRenderer _line;
    protected TimeManager _timeManager;
    protected PoolManager _poolManager;
    protected GrapplingGun _grapplingGun;
    protected AimIndicator _aimIndicator;
    //protected SimulatedSoundEffect _audioSimulator;

    protected AnimationCurve _lineWidth;

    protected virtual float _fadeSpeed => .5f;
    protected const int MAX_VERTEX = 300; //50
    protected const float LENGTH = .01f;

    public PoolableType PoolableType => PoolableType.None;
    protected virtual void Awake()
    {
        _timeManager = TimeManager.Instance;
        _poolManager = PoolManager.Instance;
        _grapplingGun = Hero.Instance.GrapplingGun;
        _line = GetComponent<LineRenderer>();
        _lineWidth = _line.widthCurve;
        _line.positionCount = 2;
        Color = CustomColor.Blue;
    }

    protected virtual void Update()
    {
        var color = Aiming ? ColorUtils.Red : ColorUtils.Grey;
        _line.startColor = color;
        _line.endColor = color;
    }

    protected virtual void OnEnable()
    {
        Active = true;
    }

    public void DrawTrajectory(Vector2 linePosition, Vector2 direction)
    {
        _line.SetPosition(0, new Vector3(linePosition.x, linePosition.y, 0));

        var targetPosition = linePosition - direction.normalized * GrapplingGun.CHARGE_LENGTH;
        var chargeHit = StepClear(linePosition, targetPosition - linePosition, GrapplingGun.CHARGE_LENGTH);

        HandleTrajectoryHit(chargeHit, linePosition, ref targetPosition);

        _line.SetPosition(1, targetPosition);
    }

    private bool HandleTrajectoryHit(RaycastHit2D hit, Vector2 linePosition, ref Vector2 chargePos)
    {
        if (!hit)
        {
            DeactivateAim();
            Target = null;
            return false;
        }

        if (!HandleFocusableCast(hit, ref chargePos))
        {
            hit = StepClearWall(linePosition, chargePos - linePosition, GrapplingGun.CHARGE_LENGTH);
            SetAudioSimulator(_line.GetPosition(1), 5);
            DeactivateAim();
            if (hit)
            {
                chargePos = hit.point;
            }
        }

        return true;
    }

    private bool HandleFocusableCast(RaycastHit2D hit, ref Vector2 chargePos)
    {
        if (!hit.collider.CompareTag("Interactive") && !hit.collider.CompareTag("Enemy"))
            return false;

        if (!hit.collider.TryGetComponent(out IFocusable focusable))
        {
            focusable = hit.collider.GetComponentInParent<IFocusable>();

            if (focusable == null)
                return false;
        }

        if (focusable.Taken)
            return false;

        Target = focusable;
        chargePos = focusable.Transform.position;
        ActivateAim(focusable, chargePos);
        _aimIndicator.Transform.position = chargePos;

        if (!focusable.IsSilent) SetAudioSimulator(_line.GetPosition(1), 5);

        return true;
    }

    protected virtual RaycastHit2D StepClear(Vector3 origin, Vector3 direction, float distance)
    {
        // Readapt radius if hero scale changes (otherwise cast hits the ground behind hero)
        return Physics2D.CircleCast(origin, .7f, direction, distance,
                    (1 << LayerMask.NameToLayer("Obstacle")) |
                    (1 << LayerMask.NameToLayer("DynamicObstacle")) |
                    (1 << LayerMask.NameToLayer("Enemy")) |
                    (1 << LayerMask.NameToLayer("Interactive")) |
                    (1 << LayerMask.NameToLayer("Teleporter")));
    }

    protected virtual RaycastHit2D StepClearWall(Vector3 origin, Vector3 direction, float distance)
    {
        // Readapt radius if hero scale changes (otherwise cast hits the ground behind hero)
        return Physics2D.CircleCast(origin, .7f, direction, distance,
                    (1 << LayerMask.NameToLayer("Obstacle")) |
                    (1 << LayerMask.NameToLayer("DynamicObstacle")));
    }

    public virtual void Disable()
    {
        Used = false;
        //StartCoroutine(FadeAway());
        _timeManager.SetNormalTime();
        DeactivateAim();
        Active = false;
        Sleep();
    }

    public virtual void ReUse(Vector3 position)
    {
        Transform.position = new Vector3(position.x, position.y, -5);
        Appear();
    }

    protected virtual void Appear()
    {
        if (Used)
            return;

        if (!Hero.Instance.Stickiness.Attached)
        {
            _timeManager.SlowDown();
            _timeManager.StartTimeRestore();
        }

        Color col = _line.material.color;
        _line.material.color = new Color(col.r, col.g, col.b, 1);

        Used = true;
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        Transform.position = new Vector3(position.x, position.y, -5);
        Transform.rotation = rotation;
        Appear();
    }

    public void Sleep()
    {
        gameObject.SetActive(false);
    }

    public void Wake()
    {
        gameObject.SetActive(true);
    }

    public void SetAudioSimulator(Vector3 position, float size)
    {
        //if (Utils.IsNull(_audioSimulator))
        //{
        //    _audioSimulator = _poolManager.GetPoolable<SimulatedSoundEffect>(position, Quaternion.identity, size);
        //}

        //_audioSimulator.Transform.position = position;
        //_audioSimulator.Transform.localScale = size * Vector2.one;
    }

    protected virtual void ActivateAim(IFocusable focusable, Vector3 position)
    {
        if (_aimIndicator != null)
            return;

        _aimIndicator = _poolManager.GetPoolable<AimIndicator>(position, Quaternion.identity);
    }

    protected virtual void DeactivateAim()
    {
        if (_aimIndicator == null)
            return;

        _aimIndicator.Sleep();
        _aimIndicator = null;
    }

    protected virtual void ResetWidths()
    {
        _line.widthCurve = _lineWidth;
    }

    public void DoReset()
    {
        Sleep();
    }
}