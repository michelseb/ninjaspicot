using System.Linq;
using UnityEngine;

public enum TouchType
{
    Left,
    Right
}

public class TouchManager : MonoBehaviour
{
    [SerializeField] private bool _mobileTouch;
    [SerializeField] private Joystick _joystick1;
    [SerializeField] private Joystick _joystick2;

    public bool WalkTouching => IsTouching(TouchType.Left);
    public bool JumpTouching => IsTouching(TouchType.Right);
    public bool WalkBegin => WalkTouching && !_walkInitialized;
    public bool WalkEnd => !WalkTouching && _walkInitialized;
    public bool Touching => WalkTouching || JumpTouching;
    public bool WalkDragging => _walkDragInitialized || (_joystick1 != null && _joystick1.Direction.magnitude > .2f);
    public bool JumpDragging => _jumpDragInitialized || (_joystick2 != null && _joystick2.Direction.magnitude > .2f);
    public bool JumpStart => JumpTouching && !_jumpInitialized;
    public bool JumpEnd => !JumpTouching && _jumpInitialized;
    public bool DoubleTouching => JumpTouching && WalkTouching;
    public bool Dashing => JumpDragging && DoubleTouching;
    public bool Running => WalkDragging && DoubleTouching;
    public bool RunStart => Running && !_runInitialized;
    public bool DashStart => Dashing && !_dashInitialized;
    private Vector3? LeftTouch => GetTouch(TouchType.Left);
    private Vector3? RightTouch => GetTouch(TouchType.Right);

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private Hero _hero;
    private Stickiness _stickiness;
    private Jumper _jumper;
    private DynamicInteraction _dynamicInteraction;
    private PoolManager _poolManager;
    private Camera _uiCamera;
    private Transform _canvasTransform;
    private bool _walkInitialized;
    private bool _walkDragInitialized;
    private bool _jumpInitialized;
    private bool _jumpDragInitialized;
    private bool _runInitialized;
    private bool _dashInitialized;
    private bool _doubleTapInitialized;

    private void Awake()
    {
        _poolManager = PoolManager.Instance;
    }

    private void Start()
    {
        _canvasTransform = UICamera.Instance.Canvas.transform;
        _uiCamera = UICamera.Instance.Camera;
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _jumper = _hero?.Jumper;
        _dynamicInteraction = _hero?.DynamicInteraction;
    }

    private void Update()
    {
        if (!HeroSpawned())
            return;

        _stickiness = (_dynamicInteraction != null && _dynamicInteraction.Interacting) ? _dynamicInteraction.CloneHeroStickiness : Hero.Instance.Stickiness;

        HandleDoubleTapInitEvent();
        HandleWalkTouchInitEvent();
        HandleJumpTouchInitEvent();
        if (!HandleJumpTouchEvent())
        {
            HandleWalkTouchEvent();
        }
        HandleDoubleTapEvent();
        HandleDoubleTapEndEvent();
        HandleWalkTouchEndEvent();
        HandleJumpTouchEndEvent();
    }

    private void LateUpdate()
    {
        if (!HeroSpawned())
            return;

        if (!JumpDragging && !Dashing && _jumper.TrajectoryInUse())
        {
            _jumper.CancelJump();
            _jumpDragInitialized = false;
            _dashInitialized = false;
        }
    }

    private bool HandleWalkTouchInitEvent()
    {
        if (!WalkBegin)
            return false;

        var touchPos = _uiCamera.ScreenToWorldPoint(LeftTouch.Value);
        _joystick1 = _poolManager.GetPoolable<Joystick>(touchPos, Quaternion.identity, 1, PoolableType.Touch1, _canvasTransform, false);
        _joystick1.OnPointerDown();
        _walkInitialized = true;

        return true;
    }

    private bool HandleWalkTouchEvent()
    {
        if (!WalkTouching)
            return false;

        // If already dragging
        if (!WalkBegin)
        {
            _joystick1.Drag(LeftTouch.Value, true);
        }

        return HandleWalkEvent();
    }

    private bool HandleWalkEvent()
    {
        if (!WalkDragging)
            return false;

        _walkDragInitialized = true;

        if (!_stickiness.Attached || !_stickiness.CanWalk)
            return false;

        HandleRunStartEvent();

        _stickiness.StartWalking();

        return true;
    }

    private void HandleRunStartEvent()
    {
        if (!RunStart)
            return;

        _stickiness.StartRunning();
        _hero.StartDisplayGhosts();

        _runInitialized = true;
    }

    private bool HandleWalkTouchEndEvent()
    {
        if (!WalkEnd)
            return false;

        _joystick1.StartFading();
        _stickiness.StopWalking(true);
        _joystick1.OnPointerUp();
        _stickiness.ReinitSpeed();
        _hero.StopDisplayGhosts();
        _runInitialized = false;
        _walkDragInitialized = false;
        _walkInitialized = false;

        return true;
    }

    private bool HandleJumpTouchInitEvent()
    {
        if (!JumpStart)
            return false;

        var touchPos = _uiCamera.ScreenToWorldPoint(RightTouch.Value);
        _joystick2 = _poolManager.GetPoolable<Joystick>(touchPos, Quaternion.identity, 1, PoolableType.Touch2, _canvasTransform, false);
        _joystick2.OnPointerDown();
        _jumpInitialized = true;

        return true;
    }

    private bool HandleJumpTouchEvent()
    {
        if (!JumpTouching)
            return false;

        // If already jump touching
        if (!JumpStart)
        {
            _joystick2.Drag(RightTouch.Value);
        }

        return HandleJumpEvent();
    }

    private bool HandleJumpEvent()
    {
        if (!JumpDragging)
            return false;

        if (!_jumper.CanInitJump())
        {
            if (_jumper.TrajectoryInUse()) _jumper.CancelJump();
            return false;
        }

        if (!_jumpDragInitialized)
        {
            HandleTrajectoryInit();
            _dashInitialized = false;
            _jumpDragInitialized = true;
        }

        _jumper.Trajectory.DrawTrajectory(_hero.Transform.position, _joystick2.Direction);

        return true;
    }

    private bool HandleJumpTouchEndEvent()
    {
        if (!JumpEnd)
            return false;

        DoJump();

        _joystick2.StartFading();
        _joystick2.OnPointerUp();
        _stickiness.ReinitSpeed();
        _hero.StopDisplayGhosts();
        _jumpInitialized = false;
        _jumpDragInitialized = false;
        _dashInitialized = false;

        return true;
    }

    private bool HandleDoubleTapInitEvent()
    {
        if (!DoubleTouching)
            return false;

        _doubleTapInitialized = true;

        return true;
    }

    private bool HandleDoubleTapEvent()
    {
        if (!_doubleTapInitialized)
            return false;

        _doubleTapInitialized = _doubleTapInitialized && !_jumpDragInitialized && !_walkDragInitialized;

        return true;
    }

    private bool HandleDoubleTapEndEvent()
    {
        if (!_doubleTapInitialized || WalkTouching || JumpTouching)
            return false;

        DoRelease();

        HandleJumpTouchEndEvent();
        HandleWalkTouchEndEvent();

        _doubleTapInitialized = false;

        return true;
    }

    private void DoJump()
    {
        if (!_jumper.ReadyToJump())
            return;

        _stickiness.StopWalking(false);

        if (_jumper.Trajectory.Target != null)
        {
            _jumper.Charge(-_joystick2.Direction);
        }
        else
        {
            _jumper.LaunchJump(_jumper.AimPosition);
        }
    }

    private void DoRelease()
    {
        _stickiness.StopWalking(false);
        _stickiness.Detach();
        _stickiness.Rigidbody.AddForce(Quaternion.Euler(0, 0, 90) * _stickiness.CollisionNormal * 500);
    }

    private void HandleTrajectoryInit()
    {
        var trajectory = _jumper.SetTrajectory();
        _joystick2.ChangeColor(trajectory.Color);
        trajectory.SetJumper(_jumper);
    }

    private bool HeroSpawned()
    {
        //Waiting for hero to spawn
        if (_hero == null)
        {
            _hero = Hero.Instance;

            if (_hero == null)
                return false;

            _stickiness = _hero.Stickiness;
            _jumper = _hero.Jumper;
            _dynamicInteraction = _hero.DynamicInteraction;
        }

        return true;
    }

    public Vector3 GetWalkDirection()
    {
        return _joystick1.Direction;
    }

    private Vector3? GetTouch(TouchType touchType)
    {
        if (!_mobileTouch && Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (IsTouching(touchType))
                return Input.mousePosition;

            return null;
        }

        var touches = Input.touches;
        if (touches.Count() == 0)
            return null;

        foreach (var touch in touches)
        {
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended)
                continue;

            if (touchType == TouchType.Left && touch.position.x <= Screen.width / 2 || touchType == TouchType.Right && touch.position.x > Screen.width / 2)
                return touch.position;
        }

        return null;
    }

    private bool IsTouching(TouchType touchType)
    {
        if (!_mobileTouch && Application.platform == RuntimePlatform.WindowsEditor)
        {
            return touchType == TouchType.Left ? Input.GetButton("Fire1") : Input.GetButton("Fire2");
        }
        else
        {
            return touchType == TouchType.Left ? LeftTouch != null : RightTouch != null;
        }
    }
}
