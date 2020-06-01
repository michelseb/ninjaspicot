using System.Collections;
using UnityEngine;

public class TouchManager : MonoBehaviour
{

    public bool Touching => Input.touchCount > 0 || Input.GetButton("Fire1");
    public bool Dragging { get; private set; }
    public Vector3 TouchOrigin { get; private set; }
    public Vector3 RawTouchOrigin { get; private set; }
    public Vector3 TouchDrag { get; private set; }

    private bool _touchInitialized;
    private int _segments;
    private float _xradius;
    private float _yradius;
    private LineRenderer _touchLine;
    private Camera _camera;
    private Coroutine _drawTouch;

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private const float DRAG_THRESHOLD = 50;

    private void Awake()
    {
        _camera = CameraBehaviour.Instance.Camera;
        _touchLine = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _touchLine.positionCount = 0;
        _touchLine.useWorldSpace = false;
    }

    private void Update()
    {

        if (Touching)
        {
            if (!_touchInitialized)
            {
                RawTouchOrigin = Input.mousePosition;
                TouchDrag = RawTouchOrigin;
                TouchOrigin = _camera.ScreenToWorldPoint(RawTouchOrigin);
                StartTouchCircle();
                _touchInitialized = true;
            }

            TouchDrag = Input.mousePosition;
        }

        if (!Touching && _touchInitialized)
        {
            _touchInitialized = false;
        }

        Dragging = IsDragging();
    }


    public void StartTouchCircle()
    {
        if (_drawTouch != null)
            return;

        _drawTouch = StartCoroutine(DrawTouchCircle(TouchOrigin));
    }


    private bool IsDragging()
    {
        if (RawTouchOrigin == null || TouchDrag == null)
            return false;

        return Vector2.Distance(RawTouchOrigin, TouchDrag) > DRAG_THRESHOLD;
    }

    public IEnumerator DrawTouchCircle(Vector3 mousePos)
    {
        Vector3 pos = _camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        pos -= transform.position;
        float x;
        float y;
        float z = 5f;
        float angle = 20f;

        for (int i = 0; i < (_segments + 1); i++)
        {
            _touchLine.positionCount = i + 1;
            x = pos.x + Mathf.Sin(Mathf.Deg2Rad * angle) * _xradius;
            y = pos.y + Mathf.Cos(Mathf.Deg2Rad * angle) * _yradius;

            _touchLine.SetPosition(i, new Vector3(x, y, z));

            angle += (360f / _segments);
            yield return new WaitForSeconds(.001f);
        }

        _drawTouch = null;
    }

    public void ReinitDrag()
    {
        TouchDrag = RawTouchOrigin;
    }

    public void Erase()
    {
        if (_drawTouch != null)
        {
            StopCoroutine(_drawTouch);
        }

        _touchLine.positionCount = 0;
    }
}
