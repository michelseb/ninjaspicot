using System.Collections;
using UnityEngine;

public class Trajectory : MonoBehaviour
{
    private LineRenderer _line;
    private int _lineMax, _targetLineCount;
    private TimeManager _timeManager;
    private Coroutine _disappearing;
    private bool _appeared;

    private const int VERTEX_LIMIT = 40;

    private static Trajectory _instance;
    public static Trajectory Instance { get { if (_instance == null) _instance = FindObjectOfType<Trajectory>(); return _instance; } }

    private void Start()
    {
        _line = Hero.Instance?.GetComponent<LineRenderer>();
        _timeManager = TimeManager.Instance;
        _lineMax = VERTEX_LIMIT;
    }

    public void DrawTrajectory(Vector2 startPos, Vector2 click, Vector2 startClick, float speed)
    {
        if (_line == null)
        {
            _line = Hero.Instance?.GetComponent<LineRenderer>();
            if (_line == null)
                return;
        }
        Vector2 grav = new Vector2(Physics2D.gravity.x, Physics2D.gravity.y);
        Vector2 pos = startPos;
        Vector2 strength = startClick - click;
        Vector2 vel = strength.normalized * speed;
        if (_targetLineCount > 2)
        {
            Appear();
        }
        if (_disappearing != null)
        {
            StopCoroutine(FadeAway());
            _disappearing = null;
        }

        if (_targetLineCount < _lineMax + 3)
        {
            _targetLineCount += 2;
        }
        if (_targetLineCount > _lineMax - 2)
        {
            _targetLineCount = _lineMax;
        }
        _line.positionCount = _targetLineCount;
        _lineMax = VERTEX_LIMIT;


        for (var i = 0; i < _targetLineCount; i++)
        {
            vel = vel + grav * Time.fixedUnscaledDeltaTime; //* power;
            pos = pos + (vel * Time.fixedUnscaledDeltaTime); //* power);
            _line.SetPosition(i, new Vector3(pos.x, pos.y, 0));
            if (i > 1)
            {
                RaycastHit2D hit = Physics2D.CircleCast(_line.GetPosition(i - 1), 1, _line.GetPosition(i - 2) - _line.GetPosition(i - 1), .1f, LayerMask.GetMask("Default"));

                if (hit)
                {
                    if (hit.collider.gameObject.GetComponent<Wall>() != null)
                    {
                        _lineMax = i;
                        _targetLineCount = i;
                        _line.positionCount = _targetLineCount;
                        break;
                    }

                }

            }
        }
    }

    public bool IsClear(Vector3 origin, int lineIndex, int layer = 0)
    {
        if (_targetLineCount < 1)
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

    public void Reduce()
    {
        if (_line.widthMultiplier > 0)
        {
            _line.widthMultiplier -= .01f;
        }

    }

    public void Reset()
    {
        _line.widthMultiplier = 2;
    }

    public void ReinitTrajectory()
    {
        _appeared = false;
        _disappearing = StartCoroutine(FadeAway());
    }


    private IEnumerator FadeAway()
    {
        _timeManager.SetNormalTime();
        _targetLineCount = 0;
        _lineMax = VERTEX_LIMIT;
        Color col = _line.material.color;
        while (col.a > 0)
        {
            col = _line.material.color;
            col.a -= 0.01f;
            _line.material.color = col;
            yield return null;
        }
        _disappearing = null;
    }

    private void Appear()
    {
        if (!_appeared)
        {
            _timeManager.SlowDown(.05f);
            _timeManager.StartTimeRestore();
        }

        _appeared = true;
        Color col = _line.material.color;
        col.a = 1f;
        _line.material.color = col;

        //StartCoroutine(cam.zoomOut(60));
    }
}