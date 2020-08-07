using System.Collections;
using UnityEngine;

public abstract class TrajectoryBase : MonoBehaviour, IPoolable
{
    public bool Used { get; private set; }
    public bool Active { get; private set; }
    public float Strength { get; set; }

    protected LineRenderer _line;
    protected Transform _transform;
    protected TimeManager _timeManager;

    protected const float FADE_SPEED = .5f;
    protected const int MAX_VERTEX = 50;
    protected const float LENGTH = .01f;

    public PoolableType PoolableType => PoolableType.None;

    protected virtual void Awake()
    {
        _transform = transform;
        _timeManager = TimeManager.Instance;
        _line = GetComponent<LineRenderer>();
    }

    protected virtual void OnEnable()
    {
        Active = true;
    }

    public abstract void DrawTrajectory(Vector2 linePosition, Vector2 click, Vector2 startClick);

    protected virtual RaycastHit2D StepClear(Vector3 origin, Vector3 direction, float distance)
    {
        return Physics2D.CircleCast(origin, 1, direction, distance,
                    (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));
    }

    public virtual bool IsClear(Vector3 origin, int lineIndex, int layer = 0)
    {
        if (_line.positionCount < 1)
            return false;

        for (int i = 0; i < lineIndex; i++)
        {
            var pos = _line.GetPosition(i);
            RaycastHit2D hit = Physics2D.Linecast(origin, pos, layer != 0 ? layer : LayerMask.GetMask("Default"));

            if (hit && !hit.collider.CompareTag("hero") && !hit.collider.isTrigger)
                return false;
        }

        return true;
    }

    public void StartFading()
    {
        Used = false;
        StartCoroutine(FadeAway());
    }


    protected virtual IEnumerator FadeAway()
    {
        _timeManager.SetNormalTime();
        Color col = _line.material.color;
        while (col.a > 0)
        {
            col = _line.material.color;
            col.a -= Time.deltaTime * FADE_SPEED;
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

        _timeManager.SlowDown();
        _timeManager.StartTimeRestore();

        Color col = _line.material.color;
        _line.material.color = new Color(col.r, col.g, col.b, 1);

        Used = true;
    }

    public void Pool(Vector3 position, Quaternion rotation)
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
}