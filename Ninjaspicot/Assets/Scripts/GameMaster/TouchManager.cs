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
    [SerializeField] private bool _mobileTouch;

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
    private TimeManager _timeManager;

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private Hero _hero;
    private Stickiness _stickiness;
    private HeroJumper _jumpManager;
    private DynamicInteraction _dynamicInteraction;

    private TextMeshProUGUI _debugText;
    private const float DRAG_THRESHOLD = 150;

    private void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _camera = _cameraBehaviour.Camera;
        _touchLine = GetComponent<LineRenderer>();
        _poolManager = PoolManager.Instance;
        _timeManager = TimeManager.Instance;
        _debugText = _cameraBehaviour.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        _touchLine.positionCount = 0;
        _touchLine.useWorldSpace = true;
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _jumpManager = _hero?.JumpManager as HeroJumper;
        _dynamicInteraction = _hero?.DynamicInteraction;
        InitTouchIndexes();
    }

    private void Update()
    {
        var touch1 = Input.touches.FirstOrDefault(x => x.fingerId == _index0);
        var touch2 = Input.touches.FirstOrDefault(x => x.fingerId == _index1);
        _debugText.text = "Touch" + _index0 + " : " + touch1.phase + " - Touch" + _index1 + " : " + touch2.phase;

        //Waiting for hero to spawn
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _jumpManager = _hero?.JumpManager as HeroJumper;
            _stickiness = _hero?.Stickiness;
            _dynamicInteraction = _hero?.DynamicInteraction;
            if (_hero == null)
                return;
        }

        _stickiness = _dynamicInteraction.Interacting ? _dynamicInteraction.CloneHeroStickiness : Hero.Instance?.Stickiness;

        if (Touching)
        {
            if (!_touchInitialized)
            {
                Debug.Log("Init");
                InitTouchIndexes();

                RawTouch1Origin = GetRawTouch(_index0);

                ReinitDrag1();
                Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
                _touch1Indicator = _poolManager.GetPoolable<TouchIndicator>(Touch1Origin, Quaternion.identity, PoolableType.Touch1, _camera.transform);
                _touchInitialized = true;
            }
            else
            {
                var drag = GetDrag(_index0);
                Touch2Drag = drag ?? Touch1Drag;
                Dragging1 = IsDragging1(true);
                Debug.Log("origin : " + RawTouch1Origin + " - drag : " + Touch1Drag);
                if (Dragging1)
                {
                    _jumpManager.SetJumpPositions(RawTouch1Origin, Touch1Drag);
                }
            }

        }

        if (DoubleTouching)
        {
            if (!_touch2Initialized)
            {
                RawTouch2Origin = GetRawTouch(_index1);

                ReinitDrag2();
                Touch2Origin = _camera.ScreenToWorldPoint(RawTouch2Origin);
                _touch2Indicator = _poolManager.GetPoolable<TouchIndicator>(Touch2Origin, Quaternion.identity, PoolableType.Touch2, _camera.transform);

                //Hero effects
                _stickiness.CurrentSpeed *= 2;
                _hero.StartDisplayGhosts();

                _touch2Initialized = true;
            }
            else
            {
                var drag = GetDrag(_index1);
                Touch2Drag = drag ?? Touch2Drag;
                Dragging2 = IsDragging2(true);
                if (Dragging2)
                {
                    _jumpManager.SetJumpPositions(RawTouch2Origin, Touch2Drag);
                }
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
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended)
                {
                    if (_jumpManager.NeedsJump1)
                    {
                        _jumpManager.SetJumpPositions(RawTouch1Origin, Touch1Drag);
                    }
                    else if (_jumpManager.NeedsJump2)
                    {
                        _jumpManager.SetJumpPositions(RawTouch2Origin, Touch2Drag);
                    }
                    Debug.Log("Switch");
                    Debug.Log("origin : " + RawTouch1Origin + " - drag : " + Touch1Drag);
                    SwitchTouchIndexes();
                    var tempDrag = Touch1Drag;
                    var tempTouch = RawTouch1Origin;
                    RawTouch1Origin = RawTouch2Origin;
                    Touch1Drag = Touch2Drag;
                    RawTouch2Origin = tempTouch;
                    Touch2Drag = tempDrag;
                    Dragging1 = IsDragging1(false);
                    Debug.Log("new origin : " + RawTouch1Origin + " - drag : " + Touch1Drag);

                    _jumpManager.NeedsJump1 = _jumpManager.NeedsJump1 || _jumpManager.NeedsJump2;


                    Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
                    _touch1Indicator.transform.position = Touch1Origin;
                }
            }

            Dragging2 = false;
            _touch2Initialized = false;
        }

        if (!Touching && _touchInitialized)
        {
            _touch1Indicator.StartFading();
            Dragging1 = false;
            _touchInitialized = false;
        }
    }


    private void LateUpdate()
    {
        if (Touching)
        {
            _stickiness.NinjaDir = TouchArea == TouchArea.Left ? Dir.Left : Dir.Right;

            if (_jumpManager.CanJump())
            {
                _jumpManager.SetTrajectory();
                _jumpManager.Trajectory.DrawTrajectory(_hero.transform.position, _jumpManager.TrajectoryDestination, _jumpManager.TrajectoryOrigin);
            }
            else if (_jumpManager.TrajectoryInUse())
            {
                _jumpManager.ReinitJump();
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

        if (Input.GetButtonUp("Fire1") || TouchLifted)
        {
            if (_jumpManager.ReadyToJump())
            {
                if (_jumpManager.TrajectoryInUse() && !_jumpManager.Trajectory.IsClear(transform.position, 2))//Add ninja to new layer
                {
                    _jumpManager.Trajectory.StartFading();
                    _timeManager.SetNormalTime();
                }
                else
                {
                    _stickiness.StopWalking(false);

                    _jumpManager.Jump(_jumpManager.TrajectoryOrigin, _jumpManager.TrajectoryDestination);
                    ReinitDrags();
                }
            }
        }
    }

    private bool IsDragging1(bool setJump)
    {
        var dragging = Touching && Vector2.Distance(RawTouch1Origin, Touch1Drag) > DRAG_THRESHOLD;
        if (setJump)
        {
            _jumpManager.NeedsJump1 = dragging;
        }

        return dragging;
    }

    private bool IsDragging2(bool setJump)
    {
        var dragging = DoubleTouching && Vector2.Distance(RawTouch2Origin, Touch2Drag) > DRAG_THRESHOLD;
        if (setJump)
        {
            _jumpManager.NeedsJump2 = dragging;
        }

        return dragging;
    }

    public void ReinitDrag1()
    {
        Touch1Drag = RawTouch1Origin;
        Dragging1 = false;
    }

    public void ReinitDrag2()
    {
        Touch2Drag = RawTouch2Origin;
        Dragging2 = false;
    }

    public void ReinitDrags()
    {
        ReinitDrag1();
        ReinitDrag2();
    }

    private Vector3 GetRawTouch(int index)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && !_mobileTouch)
            return Input.mousePosition;

        var touch = Input.touches.FirstOrDefault(t => t.fingerId == index);

        if (touch.phase == TouchPhase.Ended || touch.position == Vector2.zero)
            return index == 0 ? RawTouch1Origin : RawTouch2Origin;

        return touch.position;
    }

    private Vector3? GetDrag(int index)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && !_mobileTouch)
            return Input.mousePosition;

        var touch = Input.touches.FirstOrDefault(t => t.fingerId == index);

        if (touch.phase == TouchPhase.Ended || touch.position == Vector2.zero)
            return null;

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
