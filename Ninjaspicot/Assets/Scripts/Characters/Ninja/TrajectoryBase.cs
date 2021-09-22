using System.Collections;
using UnityEngine;

public abstract class TrajectoryBase : MonoBehaviour, IPoolable
{
    public bool Used { get; private set; }
    public bool Active { get; private set; }
    public float Strength { get; set; }

    public abstract CustomColor Color { get; }
    public abstract JumpMode JumpMode { get; }

    protected LineRenderer _line;
    protected Transform _transform;
    protected TimeManager _timeManager;
    protected PoolManager _poolManager;
    protected Jumper _jumper;

    protected SimulatedSoundEffect _audioSimulator;

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
    }

    protected virtual void OnEnable()
    {
        Active = true;
    }

    public abstract void DrawTrajectory(Vector2 linePosition, Vector2 direction);

    protected virtual RaycastHit2D StepClear(Vector3 origin, Vector3 direction, float distance)
    {
        return Physics2D.CircleCast(origin, 1, direction, distance,
                    (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));
    }

    public void SetJumper(Jumper jumper)
    {
        _jumper = jumper;
    }

    public void StartFading()
    {
        Used = false;
        StartCoroutine(FadeAway());
    }

    public Vector3 GetLinePosition(int index)
    {
        index = Mathf.Min(index, _line.positionCount - 1);
        return _line.GetPosition(index);
    }


    protected virtual IEnumerator FadeAway()
    {
        _audioSimulator?.Deactivate();
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
        Deactivate();
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

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void SetAudioSimulator(Vector3 position, float size)
    {
        if (Utils.IsNull(_audioSimulator))
        {
            _audioSimulator = _poolManager.GetPoolable<SimulatedSoundEffect>(position, Quaternion.identity);
        }

        _audioSimulator.Transform.position = position;
        _audioSimulator.Transform.localScale = size * Vector2.one;
    }
}