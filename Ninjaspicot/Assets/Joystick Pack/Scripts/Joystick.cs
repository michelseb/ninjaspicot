using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPoolable
{
    public bool Active { get; private set; }
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    private TouchManager _touchManager;
    private Transform _transform;

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
    public bool SnapX { get { return snapX; } set { snapX = value; } }
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    public PoolableType PoolableType => _poolableType;

    [SerializeField] private PoolableType _poolableType;
    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both;
    [SerializeField] private bool snapX = false;
    [SerializeField] private bool snapY = false;
    [SerializeField] private CustomColor customColor;
    [SerializeField] protected RectTransform background = null;
    [SerializeField] private RectTransform handle = null;
    private RectTransform baseRect = null;
    private Image _image;
    private Image _handleImage;
    private float _alpha;

    private Canvas canvas;
    private Camera cam;
    private Color _initColor;
    private Coroutine _appear;

    private Vector2 input = Vector2.zero;

    private const float APPEAR_SPEED = 4f;
    private const float FADE_SPEED = 1.5f;

    protected virtual void Awake()
    {
        _image = GetComponent<Image>();
        _handleImage = handle.GetComponent<Image>();
        _touchManager = TouchManager.Instance;
        _initColor = _image.color;
        _alpha = _image.color.a;
        _transform = transform;
    }

    protected virtual void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("The Joystick is not placed inside a canvas");

        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        _touchManager.GetTouchDown(this);
        SetColor(ColorUtils.GetColor(customColor, _alpha));
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;
    }

    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
            input = Vector2.zero;
    }

    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
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

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _touchManager.GetTouchUp(this);
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
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
        while (col.a < 1)
        {
            col = _image.color;
            col.a += Time.deltaTime * APPEAR_SPEED;
            _image.color = col;
            yield return null;
        }
        _appear = null;
    }

    public void Pool(Vector3 position, Quaternion rotation)
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