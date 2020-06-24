using System.Linq;
using TMPro;
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
    public bool Dragging => Dragging1 || Dragging2;
    public bool Dragging1 { get; private set; }
    public bool Dragging2 { get; private set; }
    public Vector3 Touch1Origin { get; private set; }
    public Vector3 Touch2Origin { get; private set; }
    public Vector3 RawTouch1Origin { get; private set; }
    public Vector3 RawTouch2Origin { get; private set; }
    public Vector3 Touch1Drag { get; private set; }
    public Vector3 Touch2Drag { get; private set; }
    public bool TouchLifted => FingerLifted();
    public TouchArea TouchArea => RawTouch1Origin.x < Screen.width / 2 ? TouchArea.Left : TouchArea.Right;

    private int _index0, _index1; // Touch indexes;
    private int _touchCount;
    private bool _touchInitialized;
    private bool _touch2Initialized;
    private TouchIndicator _touch1Indicator;
    private TouchIndicator _touch2Indicator;
    private LineRenderer _touchLine;
    private Camera _camera;
    private CameraBehaviour _cameraBehaviour;
    private PoolManager _poolManager;

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private Hero _hero;
    private Stickiness _stickiness;
    private Jumper _jumpManager;
    private DynamicInteraction _dynamicInteraction;

    private TextMeshProUGUI _debugText;
    private const float DRAG_THRESHOLD = 150;

    private void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _camera = _cameraBehaviour.Camera;
        _touchLine = GetComponent<LineRenderer>();
        _poolManager = PoolManager.Instance;
        _debugText = _cameraBehaviour.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _touchLine.positionCount = 0;
        _touchLine.useWorldSpace = true;
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _jumpManager = _hero?.JumpManager;
        _dynamicInteraction = _hero?.DynamicInteraction;
        InitTouchIndexes();
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

        if (Touching)
        {
            Touch1Drag = GetRawTouch(_index0);

            if (!_touchInitialized)
            {
                InitTouchIndexes();
                Touch1Drag = GetRawTouch(_index0);
                RawTouch1Origin = Touch1Drag;
                Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
                _touch1Indicator = _poolManager.GetPoolable<TouchIndicator>(Touch1Origin, Quaternion.identity, PoolableType.Touch1, _camera.transform);
                _touchInitialized = true;
            }
        }

        if (DoubleTouching)
        {
            Touch2Drag = GetRawTouch(_index1);

            if (!_touch2Initialized)
            {
                Touch2Drag = GetRawTouch(_index1);
                RawTouch2Origin = Touch2Drag;
                Touch2Origin = _camera.ScreenToWorldPoint(RawTouch2Origin);

                //Hero effects
                _stickiness.CurrentSpeed *= 2;
                _hero.StartDisplayGhosts();
                _touch2Indicator = _poolManager.GetPoolable<TouchIndicator>(Touch2Origin, Quaternion.identity, PoolableType.Touch2, _camera.transform);
                _touch2Initialized = true;
            }
        }

        _stickiness = _dynamicInteraction.Interacting ? _dynamicInteraction.CloneHeroStickiness : Hero.Instance?.Stickiness;

        if (Touching)
        {
            _stickiness.NinjaDir = TouchArea == TouchArea.Left ? Dir.Left : Dir.Right;


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
            _touch2Indicator.StartFading();

            //Touch2 becomes touch1           
            if (Touching)
            {
                var touch = Input.touches.FirstOrDefault(t => t.fingerId == _index0);
                if (touch.phase == TouchPhase.Began)
                {
                    SwitchTouchIndexes();
                    RawTouch1Origin = GetRawTouch(_index0);

                    if (!Dragging)
                    {
                        ReinitDrags();
                    }
                    Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
                    _touch1Indicator.transform.position = Touch1Origin;
                }
            }

            _touch2Initialized = false;
        }

        if (!Touching && _touchInitialized)
        {
            _touch1Indicator.StartFading();
            _touchInitialized = false;
        }

        Dragging1 = IsDragging1();
        Dragging2 = IsDragging2();
    }

    private bool IsDragging1()
    {
        return Vector2.Distance(RawTouch1Origin, Touch1Drag) > DRAG_THRESHOLD;
    }

    private bool IsDragging2()
    {
        if (!_touch2Initialized)
            return false;

        return Vector2.Distance(RawTouch2Origin, Touch2Drag) > DRAG_THRESHOLD;
    }

    public void ReinitDrag1()
    {
        Touch1Drag = RawTouch1Origin;
    }

    public void ReinitDrag2()
    {
        Touch2Drag = RawTouch2Origin;
    }

    public void ReinitDrags()
    {
        ReinitDrag1();
        ReinitDrag2();
    }

    private Vector3 GetRawTouch(int index)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return Input.mousePosition;

        var touch = Input.touches.FirstOrDefault(t => t.fingerId == index);

        if (touch.phase == TouchPhase.Ended)
            return index == 0 ? Touch1Drag : Touch2Drag;

        return touch.position;
    }

    private bool FingerLifted()
    {
        bool fingerLifted = false;
        var touchCount = Input.touchCount;

        if (touchCount < _touchCount)
        {
            fingerLifted = true;
        }

        _touchCount = touchCount;

        return fingerLifted;
    }

    private void InitTouchIndexes()
    {
        _index0 = 0;
        _index1 = 1;
    }
    private void SwitchTouchIndexes()
    {
        _index0 = 1 - _index0;
        _index1 = 1 - _index0;
    }
}
