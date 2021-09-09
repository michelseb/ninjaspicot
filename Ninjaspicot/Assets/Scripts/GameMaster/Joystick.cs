using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPoolable
{
    public bool Active { get; private set; }
    public float Horizontal { get { return (_snapX) ? SnapFloat(_input.x, AxisOptions.Horizontal) : _input.x; } }
    public float Vertical { get { return (_snapY) ? SnapFloat(_input.y, AxisOptions.Vertical) : _input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    private Transform _transform;

    public float HandleRange
    {
        get { return _handleRange; }
        set { _handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return _deadZone; }
        set { _deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return AxisOptions; } set { _axisOptions = value; } }
    public bool SnapX { get { return _snapX; } set { _snapX = value; } }
    public bool SnapY { get { return _snapY; } set { _snapY = value; } }

    public PoolableType PoolableType => _poolableType;

    [SerializeField] private PoolableType _poolableType;
    [SerializeField] private float _handleRange = 1;
    [SerializeField] private float _deadZone = 0;
    [SerializeField] private AxisOptions _axisOptions = AxisOptions.Both;
    [SerializeField] private bool _snapX = false;
    [SerializeField] private bool _snapY = false;
    [SerializeField] private CustomColor _customColor;
    [SerializeField] protected RectTransform _background = null;
    [SerializeField] private RectTransform _handle = null;

    private Image _image;
    private Image _handleImage;
    private float _alpha;

    private Canvas _canvas;
    private Camera _cam;
    private Color _initColor;
    private Coroutine _appear;

    private Vector2 _input = Vector2.zero;

    private const float APPEAR_SPEED = 4f;
    private const float FADE_SPEED = 1.5f;

    protected virtual void Awake()
    {
        _image = GetComponent<Image>();
        _handleImage = _handle.GetComponent<Image>();
        _initColor = _image.color;
        _alpha = _image.color.a;
        _transform = transform;
    }

    protected virtual void Start()
    {
        HandleRange = _handleRange;
        DeadZone = _deadZone;
        
        _canvas = GetComponentInParent<Canvas>();

        _cam = null;
        if (_canvas?.renderMode == RenderMode.ScreenSpaceCamera)
        {
            _cam = _canvas.worldCamera;
        }

        Vector2 center = new Vector2(0.5f, 0.5f);
        _background.pivot = center;
        _handle.anchorMin = center;
        _handle.anchorMax = center;
        _handle.pivot = center;
        _handle.anchoredPosition = Vector2.zero;
        SetColor(ColorUtils.GetColor(_customColor, _alpha));
    }

    public void Drag(Vector2 touchPosition)
    {
        if (_cam == null)
            return;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(_cam, _background.position);
        Vector2 radius = _background.sizeDelta / 2;
        _input = (touchPosition - position) / (radius * _canvas.scaleFactor);
        FormatInput();
        HandleInput(_input.magnitude, _input.normalized, radius, _cam);
        _handle.anchoredPosition = _input * radius * _handleRange;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > _deadZone)
        {
            if (magnitude > 1)
                _input = normalised;
        }
        else
            _input = Vector2.zero;
    }

    private void FormatInput()
    {
        if (_axisOptions == AxisOptions.Horizontal)
            _input = new Vector2(_input.x, 0f);
        else if (_axisOptions == AxisOptions.Vertical)
            _input = new Vector2(0f, _input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (_axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(_input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    public virtual void ChangeColor(CustomColor color)
    {
        if (_customColor == color)
            return;

        _customColor = color;
        SetColor(ColorUtils.GetColor(_customColor, _alpha));
    }

    public virtual void OnPointerDown()
    {
        SetColor(ColorUtils.GetColor(_customColor, _alpha));
    }

    public virtual void OnPointerUp()
    {
        _input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
        SetColor(_initColor);
    }

    protected void SetColor(Color color)
    {
        _image.color = color;
        _handleImage.color = color;
    }

    private void OnEnable()
    {
        var col = _image.color;
        _image.color = new Color(col.r, col.g, col.b, 0);
    }

    public void StartFading()
    {
        if (_appear != null)
        {
            StopCoroutine(_appear);
        }
        StartCoroutine(FadeAway());
    }


    private IEnumerator FadeAway()
    {
        Color col = _image.color;
        while (col.a > 0)
        {
            col = _image.color;
            col.a -= Time.deltaTime * FADE_SPEED;
            _image.color = col;
            yield return null;
        }
        Deactivate();
    }

    private IEnumerator Appear()
    {
        Color col = _image.color;
        while (col.a < _alpha)
        {
            col = _image.color;
            col.a += Time.deltaTime * APPEAR_SPEED;
            _image.color = col;
            yield return null;
        }
        _appear = null;
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        _transform.position = new Vector3(position.x, position.y, -5);
        _transform.rotation = rotation;
        _appear = StartCoroutine(Appear());
    }

    public void Deactivate()
    {
        Active = false;
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        Active = true;
    }
}

public enum AxisOptions { Both, Horizontal, Vertical }