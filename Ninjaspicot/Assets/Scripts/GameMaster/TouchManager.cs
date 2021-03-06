﻿using System.Linq;
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

    public bool Touching => Input.touchCount > 0 || Input.GetButton("Fire1") || Input.GetButton("Fire2");
    public bool DoubleTouching => Input.touchCount > 1 || (Input.GetButton("Fire1") && Input.GetButton("Fire2"));
    public bool Moving => GetMoving();
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
    private HeroJumper _jumper;
    private DynamicInteraction _dynamicInteraction;

    private const float DRAG_THRESHOLD = 150;

    private void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _camera = _cameraBehaviour.Camera;
        _touchLine = GetComponent<LineRenderer>();
        _poolManager = PoolManager.Instance;
        _timeManager = TimeManager.Instance;
    }

    private void Start()
    {
        _touchLine.positionCount = 0;
        _touchLine.useWorldSpace = true;
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _jumper = _hero?.Jumper as HeroJumper;
        _dynamicInteraction = _hero?.DynamicInteraction;
        InitTouchIndexes();
    }

    private void Update()
    {
        if (!HeroSpawned())
            return;

        _stickiness = _dynamicInteraction.Interacting ? _dynamicInteraction.CloneHeroStickiness : Hero.Instance?.Stickiness;

        if (Touching)
        {
            if (!_touchInitialized)
            {
                InitTouchIndexes();

                RawTouch1Origin = GetRawTouch(_index0);

                ReinitDrag1();
                Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
                _touch1Indicator = _poolManager.GetPoolable<TouchIndicator>(Touch1Origin, Quaternion.identity, PoolableType.Touch1, _cameraBehaviour.Transform, false);
                _touchInitialized = true;
            }
            else
            {
                var drag = GetDrag(_index0);
                Touch1Drag = drag ?? Touch1Drag;
                Dragging1 = IsDragging1(true);
                if (Dragging1)
                {
                    _jumper.SetJumpPositions(RawTouch1Origin, Touch1Drag);
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
                _touch2Indicator = _poolManager.GetPoolable<TouchIndicator>(Touch2Origin, Quaternion.identity, PoolableType.Touch2, _cameraBehaviour.Transform, false);

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
                    _jumper.SetJumpPositions(RawTouch2Origin, Touch2Drag);
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
                    if (_jumper.NeedsJump1)
                    {
                        _jumper.SetJumpPositions(RawTouch1Origin, Touch1Drag);
                    }
                    else if (_jumper.NeedsJump2)
                    {
                        _jumper.SetJumpPositions(RawTouch2Origin, Touch2Drag);
                    }
                    SwitchTouchIndexes();
                    var tempDrag = Touch1Drag;
                    var tempTouch = RawTouch1Origin;
                    RawTouch1Origin = RawTouch2Origin;
                    Touch1Drag = Touch2Drag;
                    RawTouch2Origin = tempTouch;
                    Touch2Drag = tempDrag;
                    Dragging1 = IsDragging1(false);

                    _jumper.NeedsJump1 = _jumper.NeedsJump1 || _jumper.NeedsJump2;


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

    private void FixedUpdate()
    {
        if (!HeroSpawned())
            return;

        if (Touching)
        {
            _stickiness.NinjaDir = TouchArea == TouchArea.Left ? Dir.Left : Dir.Right;

            if (_jumper.CanJump())
            {
                if (Moving)
                {
                    if (DoubleTouching)
                    {
                        var trajectory = _jumper.SetTrajectory<ChargeTrajectory>();
                        trajectory.SetJumper(_jumper);
                        _jumper.JumpMode = JumpMode.Charge;
                    }
                    else
                    {
                        _jumper.SetTrajectory<ClassicTrajectory>();
                        _jumper.JumpMode = JumpMode.Classic;
                    }

                    _jumper.Trajectory.DrawTrajectory(_hero.transform.position, _jumper.TrajectoryDestination, _jumper.TrajectoryOrigin);
                }
            }
            else if (_jumper.TrajectoryInUse())
            {
                _jumper.ReinitJump();
            }

            if (_jumper.ReadyToJump())
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
    }


    private void LateUpdate()
    {
        if (TouchLifted || Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
        {
            if (_jumper.ReadyToJump())
            {
                if (_jumper.TrajectoryInUse() && !_jumper.Trajectory.IsClear(transform.position, 2))//Add ninja to new layer
                {
                    _jumper.Trajectory.StartFading();
                    _timeManager.SetNormalTime();
                }
                else
                {
                    _stickiness.StopWalking(false);

                    switch (_jumper.JumpMode)
                    {
                        case JumpMode.Classic:
                            _jumper.Jump();
                            break;
                        case JumpMode.Charge:
                            _jumper.Charge();
                            break;
                    }

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
            _jumper.NeedsJump1 = dragging;
        }

        return dragging;
    }

    private bool IsDragging2(bool setJump)
    {
        var dragging = DoubleTouching && Vector2.Distance(RawTouch2Origin, Touch2Drag) > DRAG_THRESHOLD;
        if (setJump)
        {
            _jumper.NeedsJump2 = dragging;
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

    public void UpdateTouchOrigins()
    {
        if (_touch1Indicator != null && _touch1Indicator.Active)
        {
            Touch1Origin = _camera.ScreenToWorldPoint(RawTouch1Origin);
            _touch1Indicator.transform.position = Touch1Origin;
        }
        if (_touch2Indicator != null && _touch2Indicator.Active)
        {
            Touch2Origin = _camera.ScreenToWorldPoint(RawTouch2Origin);
            _touch2Indicator.transform.position = Touch2Origin;
        }
    }

    private Vector3 GetRawTouch(int index)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && !_mobileTouch)
            return Input.mousePosition;

        return Input.touches.FirstOrDefault(t => t.fingerId == index).position;
    }

    private Vector3? GetDrag(int index)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && !_mobileTouch)
            return Input.mousePosition;

        var touch = Input.touches.FirstOrDefault(t => t.fingerId == index);

        if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended || touch.position == Vector2.zero)
            return null;

        return touch.position;
    }

    private bool GetMoving()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && !_mobileTouch)
            return Dragging;

        return Input.touches.Any(touch => touch.phase == TouchPhase.Moved);
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

    private bool HeroSpawned()
    {
        //Waiting for hero to spawn
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _jumper = _hero?.Jumper as HeroJumper;
            _stickiness = _hero?.Stickiness;
            _dynamicInteraction = _hero?.DynamicInteraction;
            if (_hero == null)
                return false;
        }

        return true;
    }
}
