﻿using System.Collections;
using UnityEngine;

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
    Center = 2
}


public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private int _beginZoom;
    public Transform Transform { get; private set; }
    public Camera Camera { get; private set; }
    public CameraMode CameraMode { get; private set; }
    private Hero _hero;
    private Stickiness _stickiness;
    private float _colorInterpolation;
    private Color _targetColor;
    private Transform _tracker;
    private Transform _transform;
    private Vector3 _movementOrigin;
    private Vector3 _movementDestination;

    //Center mode
    private float _centerStart;
    private float _centerDuration;

    private TimeManager _timeManager;
    private TouchManager _touchManager;

    private float _screenRatio;
    private float INITIAL_CAMERA_SIZE;
    private const float ZOOM_SPEED = 2f;
    private const float FOLLOW_SPEED = 2f;
    private const float COLOR_THRESHOLD = .01f;

    private static CameraBehaviour _instance;
    public static CameraBehaviour Instance { get { if (_instance == null) _instance = FindObjectOfType<CameraBehaviour>(); return _instance; } }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        _transform = transform;
        _timeManager = TimeManager.Instance;
        _touchManager = TouchManager.Instance;
        Transform = transform.parent.transform;

        Screen.orientation = ScreenOrientation.LandscapeLeft;

        _screenRatio = (float)Screen.height / Screen.width * .5f;
        INITIAL_CAMERA_SIZE = 200f * _screenRatio;
    }

    private void Start()
    {
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _tracker = _hero?.transform;
        InstantZoom(_beginZoom);
        Zoom(ZoomType.Intro);
    }

    private void Update()
    {
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _tracker = _hero?.transform;
            _stickiness = _hero?.Stickiness;
        }

        switch (CameraMode)
        {
            case CameraMode.Follow:

                Follow(_tracker, FOLLOW_SPEED);
                break;

            case CameraMode.Center:

                Center(_movementOrigin, _movementDestination, _centerDuration);
                break;
        }

        //_transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.Euler(0, 0, Utils.GetAngleFromVector(_stickiness.CollisionNormal) - 90), 100 * Time.deltaTime);

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
    private void Follow(Transform tracker, float speed)
    {
        Transform.position = Vector3.Lerp(Transform.position, new Vector3(tracker.position.x, tracker.position.y, Transform.position.z), speed * Time.deltaTime);
    }

    private void Center(Vector3 origin, Vector3 destination, float duration)
    {
        var interpolation = (Time.time - _centerStart) / duration;
        Transform.position = Vector3.Lerp(origin, destination, interpolation);
    }

    public void SetFollowMode(Transform tracker)
    {
        CameraMode = CameraMode.Follow;
        _tracker = tracker;
        _movementOrigin = Transform.position;
        _movementDestination = new Vector3(tracker.transform.position.x, tracker.transform.position.y, Transform.position.z);
    }

    public void SetCenterMode(Transform tracker, float duration)
    {
        CameraMode = CameraMode.Center;
        _tracker = tracker;
        _centerDuration = duration;
        _movementOrigin = Transform.position;
        _movementDestination = new Vector3(tracker.transform.position.x, tracker.transform.position.y, Transform.position.z);
        _centerStart = Time.time;
    }

    private void Colorize(Color color)
    {
        _colorInterpolation += Time.deltaTime * 10;
        Camera.backgroundColor = Color.Lerp(Camera.backgroundColor, color, _colorInterpolation);
    }

    private IEnumerator ZoomProgressive(int zoom)
    {
        float _initSize = Camera.orthographicSize;

        while (Mathf.Abs(Camera.orthographicSize - _initSize) < Mathf.Abs(zoom * _screenRatio))
        {
            Camera.orthographicSize -= zoom * Time.deltaTime * ZOOM_SPEED * _screenRatio;
            _touchManager.UpdateTouchOrigins();
            yield return null;
        }
    }

    private IEnumerator ReinitZoom()
    {
        var delta = INITIAL_CAMERA_SIZE - Camera.orthographicSize;

        while (Mathf.Sign(delta) * (INITIAL_CAMERA_SIZE - Camera.orthographicSize) > 0)
        {
            Camera.orthographicSize += delta * Time.deltaTime * ZOOM_SPEED * _screenRatio;
            _touchManager.UpdateTouchOrigins();
            yield return null;
        }
        Camera.orthographicSize = INITIAL_CAMERA_SIZE;
    }

    private IEnumerator ZoomIntro(float speed)
    {
        yield return new WaitForSecondsRealtime(2);

        while (Camera.orthographicSize > INITIAL_CAMERA_SIZE)
        {
            Camera.orthographicSize -= speed;
            _touchManager.UpdateTouchOrigins();
            yield return null;
        }
        SetFollowMode(_hero.transform);
        _hero.Jumper.Active = true;
    }

    private void InstantZoom(int zoom)
    {
        Camera.orthographicSize = INITIAL_CAMERA_SIZE + zoom * _screenRatio;
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
