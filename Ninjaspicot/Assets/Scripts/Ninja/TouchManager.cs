using UnityEngine;

public enum TouchArea
{
    Left = 0,
    Right = 1
}

public class TouchManager : MonoBehaviour
{

    public bool Touching => Input.touchCount > 0 || Input.GetButton("Fire1");
    public bool DoubleTouching => Input.touchCount > 1 || (Input.GetButton("Fire1") && Input.GetButton("Fire2"));
    public bool Dragging { get; private set; }
    public Vector3 Touch1Origin { get; private set; }
    public Vector3 Touch2Origin { get; private set; }
    public Vector3 RawTouch1Origin { get; private set; }
    public Vector3 RawTouch2Origin { get; private set; }
    public Vector3 TouchDrag { get; private set; }
    public TouchArea TouchArea => RawTouch1Origin.x < Screen.width / 2 ? TouchArea.Left : TouchArea.Right;

    private bool _touchInitialized;
    private bool _touch2Initialized;
    private LineRenderer _touchLine;
    private Camera _camera;
    //private Coroutine _drawTouch;
    private CameraBehaviour _cameraBehaviour;

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private Hero _hero;
    private Stickiness _stickiness;
    private Jumper _jumpManager;
    private DynamicInteraction _dynamicInteraction;

    private const float DRAG_THRESHOLD = 50;
    //private const int SEGMENTS = 12;
    //private const float RADIUS_X = 10;
    //private const float RADIUS_Y = 10;

    private void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _camera = _cameraBehaviour.Camera;
        _touchLine = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        _touchLine.positionCount = 0;
        _touchLine.useWorldSpace = true;
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _jumpManager = _hero?.JumpManager;
        _dynamicInteraction = _hero?.DynamicInteraction;
    }

    private void Update()
    {
        //Waiting for hero to spawn
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _jumpManager = _hero?.JumpManager;
            _stickiness = _hero?.Stickiness;
            _dynamicInteraction = _hero?.DynamicInteraction;
            if (_hero == null)
                return;
        }

        _stickiness = _dynamicInteraction.Interacting ? _dynamicInteraction.CloneHeroStickiness : Hero.Instance?.Stickiness;

        if (Input.GetButtonUp("Fire2"))
        {
            ReinitDrag();
        }

        if (Touching)
        {
            _stickiness.NinjaDir = TouchArea == TouchArea.Left ? Dir.Left : Dir.Right;

            if (!_touchInitialized)
            {
                RawTouch1Origin = GetRawTouch(0);
                TouchDrag = RawTouch1Origin;
                Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
                //StartTouchCircle();
                _touchInitialized = true;
            }

            TouchDrag = Input.mousePosition;

            if (DoubleTouching && !_touch2Initialized)
            {
                RawTouch2Origin = GetRawTouch(1);
                Touch2Origin = _camera.ScreenToWorldPoint(RawTouch2Origin);

                //Hero effects
                _stickiness.CurrentSpeed *= 2;
                _hero.StartDisplayGhosts();

                _touch2Initialized = true;
            }

            if (_jumpManager.ReadyToJump())
            {
                _stickiness.StopWalking(false);
            }
            else if (!Dragging && _stickiness.Attached && _stickiness.CanWalk)
            {
                _stickiness.StartWalking();
            }
        }
        else
        {
            if (_stickiness.Attached)
            {
                _stickiness.Rigidbody.velocity = new Vector2(0, 0);
            }
        }

        if (!DoubleTouching && _touch2Initialized)
        {
            // Cancel hero effects
            _stickiness.ReinitSpeed();
            _hero.StopDisplayGhosts();

            _touch2Initialized = false;
        }

        if (!Touching && _touchInitialized)
        {
            _touchInitialized = false;
        }

        Dragging = IsDragging();
    }


    //public void StartTouchCircle()
    //{
    //    if (_drawTouch != null)
    //        return;

    //    _drawTouch = StartCoroutine(DrawTouchCircle(TouchOrigin));
    //}


    private bool IsDragging()
    {
        if (RawTouch1Origin == null || TouchDrag == null)
            return false;

        return Vector2.Distance(RawTouch1Origin, TouchDrag) > DRAG_THRESHOLD;
    }

    //public IEnumerator DrawTouchCircle(Vector3 mousePos)
    //{
    //    Vector3 pos = new Vector3(mousePos.x, mousePos.y, 0);
    //    //pos -= _cameraBehaviour.Transform.position;
    //    float x;
    //    float y;
    //    float z = -5f;
    //    float angle = 20f;

    //    for (int i = 0; i < (SEGMENTS + 1); i++)
    //    {
    //        _touchLine.positionCount = i + 1;
    //        x = pos.x + Mathf.Sin(Mathf.Deg2Rad * angle) * RADIUS_X;
    //        y = pos.y + Mathf.Cos(Mathf.Deg2Rad * angle) * RADIUS_Y;

    //        _touchLine.SetPosition(i, new Vector3(x, y, z));

    //        angle += (360f / SEGMENTS);
    //        yield return new WaitForSeconds(.001f);
    //    }

    //    _drawTouch = null;
    //}

    public void ReinitDrag()
    {
        TouchDrag = RawTouch1Origin;
    }

    private Vector3 GetRawTouch(int index)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return Input.mousePosition;

        return Input.GetTouch(index).position;
    }

    //public void Erase()
    //{
    //    if (_drawTouch != null)
    //    {
    //        StopCoroutine(_drawTouch);
    //    }

    //    //_touchLine.positionCount = 0;
    //}
}
