using System.Collections;
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


public class CameraBehaviour : Dynamic
{
    [SerializeField]
    private int _beginZoom;
    public Transform ParentTransform { get; private set; }
    public Camera MainCamera { get; private set; }
    public CameraMode CameraMode { get; private set; }

    private Hero _hero;
    private Transform _heroTransform;
    private Stickiness _heroStickiness;
    private Vector3 _centerPos;
    private Vector3 _velocity;
    private Vector3 _normalOffset;

    private float _screenRatio;
    private float _initialCamSize;
    private const float ZOOM_SPEED = 2f;
    private const float FOLLOW_DELAY = 0.8f;
    private const float OFFSET_ADJUST_DELAY = 1.8f;

    private static CameraBehaviour _instance;
    public static CameraBehaviour Instance { get { if (_instance == null) _instance = FindObjectOfType<CameraBehaviour>(); return _instance; } }

    private void Awake()
    {
        MainCamera = GetComponent<Camera>();
        ParentTransform = Transform.parent.transform;

        Screen.orientation = ScreenOrientation.LandscapeLeft;

        _screenRatio = (float)Screen.height / Screen.width * .5f;
        _initialCamSize = 200f * _screenRatio;
    }

    private void Start()
    {
        _velocity = Vector3.zero;
        InstantZoom(_beginZoom);
        //SetFollowMode(_tracker);
        Zoom(ZoomType.Intro);
    }

    private void Update()
    {
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _heroTransform = _hero?.Transform;
            _heroStickiness = _hero?.Stickiness;
            ParentTransform.position = _heroTransform.position;
            var background = FindObjectOfType<Background>();
            if (background != null)
            {
                background.CenterBackground();
            }
        }

        switch (CameraMode)
        {
            case CameraMode.Follow:
                Follow(_heroTransform);
                break;

            case CameraMode.Center:
                Center(_centerPos);
                break;
        }
    }
    private void Follow(Transform tracker)
    {
        float speed;

        if (_heroStickiness.Walking || !_heroStickiness.Attached)
        {
            speed = FOLLOW_DELAY;
        }
        else
        {
            _normalOffset = Quaternion.Euler(0, 0, 90) * _heroStickiness.CollisionNormal * 25;
            speed = OFFSET_ADJUST_DELAY;
        }
        //Debug.Log(_normalOffset);
        if (!_heroStickiness.Attached || _normalOffset.magnitude > 0 && Vector3.Dot(_normalOffset, Quaternion.Euler(0, 0, 90) * _heroStickiness.CollisionNormal * 25) < .5f)
        {
            _normalOffset = Vector3.zero;
        }

        ParentTransform.position = Vector3.SmoothDamp(ParentTransform.position, tracker.position + _normalOffset, ref _velocity, speed);
    }

    public void MoveTo(Vector3 pos)
    {
        ParentTransform.position = new Vector3(pos.x, pos.y, ParentTransform.position.z);
    }

    private void Center(Vector3 centerPos)
    {
        var middle = (_heroTransform.position + centerPos) / 2;
        ParentTransform.position = Vector3.SmoothDamp(ParentTransform.position, middle, ref _velocity, FOLLOW_DELAY);
    }

    public void SetFollowMode()
    {
        CameraMode = CameraMode.Follow;
        _velocity = Vector3.zero;
    }

    public void SetCenterMode(Vector3 centerPos)
    {
        CameraMode = CameraMode.Center;
        _centerPos = new Vector3(centerPos.x, centerPos.y, ParentTransform.position.z);
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
        var delta = _initialCamSize - MainCamera.orthographicSize;

        while (Mathf.Sign(delta) * (_initialCamSize - MainCamera.orthographicSize) > 0)
        {
            MainCamera.orthographicSize += delta * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
            yield return null;
        }
        MainCamera.orthographicSize = _initialCamSize;
    }

    private IEnumerator ZoomIntro(float speed)
    {
        yield return new WaitForSecondsRealtime(2);

        while (MainCamera.orthographicSize > _initialCamSize)
        {
            MainCamera.orthographicSize -= speed;
            yield return null;
        }
        SetFollowMode();
        //_hero.Jumper.Active = true;
    }

    private void InstantZoom(float zoom)
    {
        MainCamera.orthographicSize = _initialCamSize + zoom * _screenRatio;
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
        var pos = Transform.localPosition;

        float ellapsed = 0;

        while (ellapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            Transform.localPosition = new Vector3(x, y, pos.z);

            ellapsed += Time.deltaTime;
            yield return null;
        }

        Transform.localPosition = pos;
    }
}
