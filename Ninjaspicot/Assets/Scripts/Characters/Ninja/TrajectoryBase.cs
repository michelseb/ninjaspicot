using System.Collections;
using UnityEngine;

public abstract class TrajectoryBase : MonoBehaviour, IPoolable
{
    public bool Used { get; protected set; }
    public bool Active { get; protected set; }
    public float Strength { get; set; }
    public IFocusable Focusable { get; protected set; }

    public abstract CustomColor Color { get; }
    public abstract JumpMode JumpMode { get; }

    protected LineRenderer _line;
    protected Transform _transform;
    protected TimeManager _timeManager;
    protected PoolManager _poolManager;
    protected Jumper _jumper;
    protected AimIndicator _aimIndicator;
    protected SimulatedSoundEffect _audioSimulator;

    protected AnimationCurve _lineWidth;

    protected virtual float _fadeSpeed => .5f;
    protected const int MAX_VERTEX = 300; //50
    protected const float LENGTH = .01f;

    public PoolableType PoolableType => PoolableType.None;
    protected virtual void Awake()
    {
        _transform = transform;
        _timeManager = TimeManager.Instance;
        _poolManager = PoolManager.Instance;
        _line = GetComponent<LineRenderer>();
        _lineWidth = _line.widthCurve;
    }

    protected virtual void OnEnable()
    {
        Active = true;
    }

    public abstract void DrawTrajectory(Vector2 linePosition, Vector2 direction);

    protected virtual RaycastHit2D StepClear(Vector3 origin, Vector3 direction, float distance)
    {
        // Readapt radius if hero scale changes (otherwise cast hits the ground behind hero)
        return Physics2D.CircleCast(origin, .8f, direction, distance,
                    (1 << LayerMask.NameToLayer("Obstacle")) | 
                    (1 << LayerMask.NameToLayer("DynamicObstacle")) | 
                    (1 << LayerMask.NameToLayer("Enemy")) |
                    (1 << LayerMask.NameToLayer("Interactive")) |
                    (1 << LayerMask.NameToLayer("Teleporter")));
    }

    protected virtual RaycastHit2D StepClearWall(Vector3 origin, Vector3 direction, float distance)
    {
        // Readapt radius if hero scale changes (otherwise cast hits the ground behind hero)
        return Physics2D.CircleCast(origin, .8f, direction, distance,
                    (1 << LayerMask.NameToLayer("Obstacle")) |
                    (1 << LayerMask.NameToLayer("DynamicObstacle")));
    }

    public void SetJumper(Jumper jumper)
    {
        _jumper = jumper;
    }

    public virtual void StartFading()
    {
        Used = false;
        StartCoroutine(FadeAway());
        DeactivateAim();
    }

    protected virtual IEnumerator FadeAway()
    {
        _audioSimulator?.Sleep();
        _audioSimulator = null;

        _timeManager.SetNormalTime();
        Color col = _line.material.color;
        while (col.a > 0)
        {
            col = _line.material.color;
            col.a -= Time.deltaTime * _fadeSpeed;
            _line.material.color = col;
            yield return null;
        }
        Active = false;
        Sleep();
    }

    public virtual void ReUse(Vector3 position)
    {
        _transform.position = new Vector3(position.x, position.y, -5);
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
        _transform.position = new Vector3(position.x, position.y, -5);
        _transform.rotation = rotation;
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
        if (Utils.IsNull(_audioSimulator))
        {
            _audioSimulator = _poolManager.GetPoolable<SimulatedSoundEffect>(position, Quaternion.identity, size);
        }

        _audioSimulator.Transform.position = position;
        _audioSimulator.Transform.localScale = size * Vector2.one;
    }

    protected virtual void ActivateAim(IFocusable focusable, Vector3 position)
    {
        if (_aimIndicator != null)
            return;

        Focusable = focusable;
        _aimIndicator = _poolManager.GetPoolable<AimIndicator>(position, Quaternion.identity);
    }

    protected virtual void DeactivateAim()
    {
        if (_aimIndicator == null)
            return;

        _aimIndicator.Sleep();
        _aimIndicator = null;
        Focusable = null;
    }

    protected virtual void ResetWidths()
    {
        _line.widthCurve = _lineWidth;
    }
}