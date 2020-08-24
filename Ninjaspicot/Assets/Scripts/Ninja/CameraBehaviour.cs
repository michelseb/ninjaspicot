using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public enum ZoomType
{
    Progressive = 0,
    Instant = 1,
    Intro = 2,
    Init = 3
}

public enum CameraMode
{
    None = 0,
    Follow = 1,
    Center = 2,
    Stick = 3
}


public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private int _beginZoom;
    [SerializeField]
    private Light2D _globalLight;
    [SerializeField]
    private Light2D _frontLight;
    public Transform Transform { get; private set; }
    public Camera MainCamera { get; private set; }
    public CameraMode CameraMode { get; private set; }
    public Coroutine ColorLerp { get; private set; }

    private Hero _hero;
    private float _colorInterpolation;
    private Color _baseColor;
    private Color _targetColor;
    private Transform _tracker;
    private Transform _transform;
    private Vector3 _movementOrigin;
    private Vector3 _movementDestination;
    private Vector3 _velocity;

    //Center mode
    private float _centerStart;
    private float _centerDuration;

    private TimeManager _timeManager;

    private float _screenRatio;
    private float INITIAL_CAMERA_SIZE;
    private const float ZOOM_SPEED = 2f;
    private const float FOLLOW_DELAY = .6f;
    private const float COLOR_THRESHOLD = .01f;

    private static CameraBehaviour _instance;
    public static CameraBehaviour Instance { get { if (_instance == null) _instance = FindObjectOfType<CameraBehaviour>(); return _instance; } }

    private void Awake()
    {
        MainCamera = GetComponent<Camera>();
        _timeManager = TimeManager.Instance;
        _transform = transform;
        Transform = _transform.parent.transform;

        Screen.orientation = ScreenOrientation.LandscapeLeft;

        _screenRatio = (float)Screen.height / Screen.width * .5f;
        INITIAL_CAMERA_SIZE = 200f * _screenRatio;
        var color = ColorUtils.GetColor(CustomColor.White);
        SetBaseColor(color, color, color, 0f);
    }

    private void Start()
    {
        _hero = Hero.Instance;
        _tracker = _hero?.transform;
        _velocity = Vector3.zero;
        InstantZoom(_beginZoom);
        Zoom(ZoomType.Intro);
    }

    private void Update()
    {
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _tracker = _hero?.transform;
        }

        switch (CameraMode)
        {
            case CameraMode.Follow:
                Follow(_tracker, FOLLOW_DELAY);
                break;

            case CameraMode.Center:
                Center(_movementOrigin, _movementDestination, _centerDuration);
                break;

            case CameraMode.Stick:
                Stick(_tracker.position);
                break;
        }

        var newCol = _baseColor * _timeManager.TimeScale;
        _targetColor = newCol;//new Color(Mathf.Clamp(newCol.r, .3f, 1), Mathf.Clamp(newCol.g, .3f, 1), Mathf.Clamp(newCol.b, .3f, 1));

        if (Mathf.Abs(MainCamera.backgroundColor.grayscale - _targetColor.grayscale) > COLOR_THRESHOLD && _colorInterpolation >= 1)
        {
            _colorInterpolation = 0;
        }

        if (_colorInterpolation < 1)
        {
            Colorize(_targetColor);
        }
    }
    private void Follow(Transform tracker, float speed)
    {
        Transform.position = Vector3.SmoothDamp(Transform.position, tracker.position, ref _velocity, speed);
    }

    private void Center(Vector3 origin, Vector3 destination, float duration)
    {
        var interpolation = (Time.unscaledTime - _centerStart) / duration;
        Transform.position = Vector3.Lerp(origin, destination, interpolation);
    }

    public void Teleport (Vector3 position)
    {
        Transform.position = new Vector3(position.x, position.y, Transform.position.z);
    }

    public void Stick(Vector3 tracker)
    {
        Transform.position = tracker;
    }

    public void SetFollowMode(Transform tracker)
    {
        CameraMode = CameraMode.Follow;
        _tracker = tracker;
        _tracker.position = new Vector3(tracker.transform.position.x, tracker.transform.position.y, Transform.position.z);
    }

    public void SetStickMode(Transform tracker)
    {
        CameraMode = CameraMode.Stick;
        _tracker = tracker;
        _tracker.position = new Vector3(tracker.transform.position.x, tracker.transform.position.y, Transform.position.z);
    }

    public void SetCenterMode(Transform tracker, float duration)
    {
        CameraMode = CameraMode.Center;
        _tracker = tracker;
        _centerDuration = duration;
        _movementOrigin = Transform.position;
        _movementDestination = new Vector3(tracker.transform.position.x, tracker.transform.position.y, Transform.position.z);
        _centerStart = Time.unscaledTime;
    }

    private void Colorize(Color color)
    {
        _colorInterpolation += Time.unscaledDeltaTime * 10;
        MainCamera.backgroundColor = Color.Lerp(MainCamera.backgroundColor, color, _colorInterpolation);
    }

    private IEnumerator ZoomProgressive(int zoom)
    {
        float _initSize = MainCamera.orthographicSize;

        while (Mathf.Abs(MainCamera.orthographicSize - _initSize) < Mathf.Abs(zoom * _screenRatio))
        {
            MainCamera.orthographicSize -= zoom * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
            yield return null;
        }
    }

    private IEnumerator ReinitZoom()
    {
        var delta = INITIAL_CAMERA_SIZE - MainCamera.orthographicSize;

        while (Mathf.Sign(delta) * (INITIAL_CAMERA_SIZE - MainCamera.orthographicSize) > 0)
        {
            MainCamera.orthographicSize += delta * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
            yield return null;
        }
        MainCamera.orthographicSize = INITIAL_CAMERA_SIZE;
    }

    private IEnumerator ZoomIntro(float speed)
    {
        yield return new WaitForSecondsRealtime(2);

        while (MainCamera.orthographicSize > INITIAL_CAMERA_SIZE)
        {
            MainCamera.orthographicSize -= speed;
            yield return null;
        }
        SetFollowMode(_hero.transform);
        _hero.Jumper.Active = true;
    }

    private void InstantZoom(int zoom)
    {
        MainCamera.orthographicSize = INITIAL_CAMERA_SIZE + zoom * _screenRatio;
    }

    public void Zoom(ZoomType type, int zoomAmount = 0)
    {
        StopAllCoroutines();

        switch (type)
        {
            case ZoomType.Progressive:
                StartCoroutine(ZoomProgressive(zoomAmount));
                break;

            case ZoomType.Instant:
                InstantZoom(zoomAmount);
                break;

            case ZoomType.Intro:
                StartCoroutine(ZoomIntro(ZOOM_SPEED));
                break;

            case ZoomType.Init:
                StartCoroutine(ReinitZoom());
                break;
        }
    }

    public void DoShake(float duration, float strength)
    {
        StartCoroutine(Shake(duration, strength));
    }

    private IEnumerator Shake(float duration, float strength)
    {
        var pos = _transform.localPosition;

        float ellapsed = 0;

        while (ellapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            _transform.localPosition = new Vector3(x, y, pos.z);

            ellapsed += Time.deltaTime;
            yield return null;
        }

        _transform.localPosition = pos;
    }

    public void SetBaseColor(Color fontColor, Color globalLightColor, Color frontLightColor, float duration)
    {
        if (ColorLerp != null)
        {
            StopCoroutine(ColorLerp);
            ColorLerp = null;
        }

        ColorLerp = StartCoroutine(LerpFont(_baseColor, fontColor, globalLightColor, frontLightColor, duration));
    }

    private IEnumerator LerpFont(Color init, Color goal, Color lightGoal, Color frontGoal, float duration)
    {
        var currTime = Time.unscaledTime;
        var interpolation = (Time.unscaledTime - currTime) / duration;
        var lightColor = _globalLight.color;
        var frontColor = _frontLight.color;

        while (interpolation < 1)
        {
            interpolation = (Time.unscaledTime - currTime) / duration;
            _baseColor = Color.Lerp(init, goal, interpolation);
            _globalLight.color = Color.Lerp(lightColor, lightGoal, interpolation);
            _frontLight.color = Color.Lerp(frontColor, frontGoal, interpolation);
            yield return null;
        }

        _baseColor = goal;
        ColorLerp = null;
    }
}
