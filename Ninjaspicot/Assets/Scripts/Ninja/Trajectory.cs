using System.Collections;
using UnityEngine;

public class Trajectory : MonoBehaviour, IPoolable
{
    public bool Used { get; private set; }
    public bool Active { get; private set; }
    public float Strength { get; set; }

    private int _lineMax;

    private LineRenderer _line;

    private TimeManager _timeManager;

    private const float TIME_SLOW = .01f;
    private const float FADE_SPEED = .5f;
    private const int MAX_VERTEX = 50;

    public PoolableType PoolableType => PoolableType.None;

    private void Awake()
    {
        _timeManager = TimeManager.Instance;
        _line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _lineMax = MAX_VERTEX;
    }

    private void OnEnable()
    {
        Active = true;
    }

    public void DrawTrajectory(Vector2 startPos, Vector2 click, Vector2 startClick)
    {
        Vector2 grav = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 pos = startPos;
        Vector2 strength = startClick - click;
        Vector2 vel = strength.normalized * Strength;

        if (_lineMax <= 2)
        {
            _lineMax = MAX_VERTEX;
        }

        if (_line.positionCount < _lineMax)
        {
            _line.positionCount = _lineMax;
        }

        for (var i = 0; i < _line.positionCount; i++)
        {
            vel = vel + grav * Time.fixedUnscaledDeltaTime;
            pos = pos + vel * Time.fixedUnscaledDeltaTime;
            _line.SetPosition(i, new Vector3(pos.x, pos.y, 0));

            if (i > 1)
            {
                RaycastHit2D hit = Physics2D.CircleCast(_line.GetPosition(i - 1), 1, _line.GetPosition(i - 2) - _line.GetPosition(i - 1), .1f,
                    (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));

                if (hit)
                {
                    if (hit.collider.gameObject.GetComponent<Obstacle>() != null)
                    {
                        _lineMax = i;
                        _line.positionCount = i;
                        break;
                    }
                }
                else
                {
                    _lineMax = MAX_VERTEX;
                }

            }
        }
    }

    public bool IsClear(Vector3 origin, int lineIndex, int layer = 0)
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


    private IEnumerator FadeAway()
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

    public void ReUse(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y, -5);
        Appear();
    }

    private void Appear()
    {
        if (Used)
            return;

        _timeManager.SlowDown(TIME_SLOW);
        _timeManager.StartTimeRestore();

        Color col = _line.material.color;
        _line.material.color = new Color(col.r, col.g, col.b, 1);

        Used = true;
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        Activate();
        transform.position = new Vector3(position.x, position.y, -5);
        transform.rotation = rotation;
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