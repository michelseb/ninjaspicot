using System.Collections;
using UnityEngine;

public enum ZoomType
{
    In = 0,
    Out = 1,
    Instant = 2,
    Intro = 3
}

public enum CameraMode
{
    Follow = 0,
    Center = 1
}


public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private int _beginZoom;
    public Transform Transform { get; private set; }
    public Camera Camera { get; private set; }
    public CameraMode CameraMode { get; private set; }
    private Hero _hero;
    private Vector2 _previousHeroPos, _heroPos;
    private float _positionInterpolation, _colorInterpolation;
    private Color _targetColor;
    private Transform _tracker;

    //Center mode
    private float _centerStart;
    private float _centerDuration;
    private Vector3 _centerOrigin;
    private Vector3 _centerDestination;

    private int _screenOrientation;

    private ScenesManager _scenesManager;
    private TimeManager _timeManager;

    private int _targetWidth;
    private float _pixelsToUnits;
    private const float ZOOM_SPEED = 100;
    private const float MOVEMENT_THRESHOLD = .1f;
    private const float COLOR_THRESHOLD = .01f;

    private static CameraBehaviour _instance;
    public static CameraBehaviour Instance { get { if (_instance == null) _instance = FindObjectOfType<CameraBehaviour>(); return _instance; } }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        _scenesManager = ScenesManager.Instance;
        _timeManager = TimeManager.Instance;
        Transform = transform.parent.transform;

        switch (_screenOrientation)
        {
            case 0:
                Screen.orientation = ScreenOrientation.Portrait;
                _targetWidth = 1080;
                _pixelsToUnits = 13;
                break;
            case 1:
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                _targetWidth = 1920;
                _pixelsToUnits = 15;
                break;
        }

    }

    private void Start()
    {
        _hero = Hero.Instance;
        _tracker = _hero.transform;
        InstantZoom(_beginZoom);
        if (_scenesManager.AlreadyPlayed)
        {
            ZoomIntroFast();
        }
    }

    private void Update()
    {
        _heroPos = new Vector2(_hero.Pos.position.x, _hero.Pos.position.y);

        switch (CameraMode)
        {
            case CameraMode.Follow:

                if (Vector3.Distance(_previousHeroPos, _heroPos) > MOVEMENT_THRESHOLD && _positionInterpolation >= 1)
                {
                    _positionInterpolation = 0;
                    _previousHeroPos = _heroPos;
                }

                if (_positionInterpolation < 1)
                {
                    Follow(_tracker);
                }

                break;


            case CameraMode.Center:

                if (_positionInterpolation < 1)
                {
                    Center(_centerOrigin, _centerDestination, _centerDuration);
                }

                break;
        }



        var newCol = Color.white * _timeManager.TimeScale;
        _targetColor = new Color(Mathf.Clamp(newCol.r, .3f, 1), Mathf.Clamp(newCol.g, .3f, 1), Mathf.Clamp(newCol.b, .3f, 1));

        if (Mathf.Abs(Camera.backgroundColor.grayscale - _targetColor.grayscale) > COLOR_THRESHOLD && _colorInterpolation >= 1)
        {
            _colorInterpolation = 0;
        }

        if (_colorInterpolation < 1)
        {
            Colorize(_targetColor);
        }
    }


    private void Follow(Transform tracker)
    {
        _positionInterpolation += Time.unscaledDeltaTime / 3000;
        Transform.position = Vector3.Lerp(Transform.position, new Vector3(tracker.position.x, tracker.position.y, Transform.position.z), _positionInterpolation);
    }

    private void Center(Vector3 origin, Vector3 destination, float duration)
    {
        _positionInterpolation = (Time.time - _centerStart) / duration;
        Transform.position = Vector3.Lerp(origin, destination, _positionInterpolation);
    }

    public void SetFollowMode(Transform tracker)
    {
        CameraMode = CameraMode.Follow;
        _tracker = tracker;
    }

    public void SetCenterMode(Transform tracker, float duration)
    {
        CameraMode = CameraMode.Center;
        _tracker = tracker;
        _centerDuration = duration;
        _positionInterpolation = 0;
        _centerOrigin = Transform.position;
        _centerDestination = new Vector3(tracker.transform.position.x, tracker.transform.position.y, Transform.position.z);
        _centerStart = Time.time;
    }

    private void Colorize(Color color)
    {
        _colorInterpolation += Time.deltaTime * 10;
        Camera.backgroundColor = Color.Lerp(Camera.backgroundColor, color, _colorInterpolation);
    }

    private IEnumerator ZoomIn(int zoom)
    {
        int height = Mathf.RoundToInt(_targetWidth / (float)Screen.width * Screen.height);

        while (Camera.orthographicSize > height / _pixelsToUnits / 2 - zoom)
        {

            Camera.orthographicSize--;
            yield return null;
        }

    }

    private void ZoomIntroFast()
    {
        int height = Mathf.RoundToInt(_targetWidth / (float)Screen.width * Screen.height);
        Camera.orthographicSize = height / _pixelsToUnits / 2;
        _hero.Movement.Started = true;
    }

    private IEnumerator ZoomIntro(float speed)
    {
        yield return new WaitForSecondsRealtime(2);
        int height = Mathf.RoundToInt(_targetWidth / (float)Screen.width * Screen.height);

        while (Camera.orthographicSize > height / _pixelsToUnits)
        {

            Camera.orthographicSize -= Time.unscaledDeltaTime * speed;
            yield return null;
        }
        _hero.Movement.Started = true;
    }

    private void InstantZoom(int zoom)
    {
        int height = Mathf.RoundToInt(_targetWidth / (float)Screen.width * Screen.height);
        Camera.orthographicSize = height / _pixelsToUnits / 2 + zoom;
    }

    private IEnumerator ZoomOut(int zoom)
    {
        int height = Mathf.RoundToInt(_targetWidth / (float)Screen.width * Screen.height);
        while (Camera.orthographicSize < height / _pixelsToUnits / 2 + zoom)
        {
            Camera.orthographicSize++;
            yield return null;
        }
    }

    public void Zoom(ZoomType type, int zoomAmount = 0)
    {
        StopAllCoroutines();

        switch (type)
        {
            case ZoomType.In:
                StartCoroutine(ZoomIn(zoomAmount));
                break;

            case ZoomType.Out:
                StartCoroutine(ZoomOut(zoomAmount));
                break;

            case ZoomType.Instant:
                InstantZoom(zoomAmount);
                break;

            case ZoomType.Intro:
                StartCoroutine(ZoomIntro(ZOOM_SPEED));
                break;
        }
    }

    public void DoShake(float duration, float strength)
    {
        StartCoroutine(Shake(duration, strength));
    }

    private IEnumerator Shake(float duration, float strength)
    {
        var pos = transform.localPosition;

        float ellapsed = 0;

        while (ellapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            transform.localPosition = new Vector3(x, y, pos.z);

            ellapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = pos;
    }

}
