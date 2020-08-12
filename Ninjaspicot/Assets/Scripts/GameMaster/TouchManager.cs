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

    public bool WalkTouching { get; private set; }
    public bool JumpTouching { get; private set; }
    public bool WalkDragging => _joystick1 != null && _joystick1.Direction.magnitude > .2f;
    public bool JumpDragging => _joystick2 != null && _joystick2.Direction.magnitude > .2f;
    public bool DoubleTouching { get; private set; }
    private Vector3? LeftTouch => GetTouch(TouchType.Left);
    private Vector3? RightTouch => GetTouch(TouchType.Right);
    public bool LeftTouching => IsTouching(TouchType.Left);
    public bool RightTouching => IsTouching(TouchType.Right);

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private Hero _hero;
    private Stickiness _stickiness;
    private HeroJumper _jumper;
    private DynamicInteraction _dynamicInteraction;
    private TimeManager _timeManager;
    private PoolManager _poolManager;
    private CameraBehaviour _cameraBehaviour;
    private Camera _uiCamera;
    private Transform _canvasTransform;
    private bool _walkInitialized;
    private bool _jumpInitialized;

    private void Awake()
    {
        _timeManager = TimeManager.Instance;
        _poolManager = PoolManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _canvasTransform = _cameraBehaviour.Canvas.transform;
    }

    private void Start()
    {
        _uiCamera = _cameraBehaviour.UiCamera;
        _hero = Hero.Instance;
        _stickiness = _hero?.Stickiness;
        _jumper = _hero?.Jumper as HeroJumper;
        _dynamicInteraction = _hero?.DynamicInteraction;
    }

    private void Update()
    {
        if (!HeroSpawned())
            return;

        _stickiness = _dynamicInteraction.Interacting ? _dynamicInteraction.CloneHeroStickiness : Hero.Instance?.Stickiness;


        if (!_walkInitialized && LeftTouching)
        {
            var touchPos = _uiCamera.ScreenToWorldPoint(LeftTouch.Value);
            _joystick1 = _poolManager.GetPoolable<Joystick>(touchPos, Quaternion.identity, PoolableType.Touch1, _canvasTransform, false);
            _joystick1.OnPointerDown();
            WalkTouching = true;
            _walkInitialized = true;
        }

        if (!_jumpInitialized && RightTouching)
        {
            var touchPos = _uiCamera.ScreenToWorldPoint(RightTouch.Value);
            _joystick2 = _poolManager.GetPoolable<Joystick>(touchPos, Quaternion.identity, PoolableType.Touch2, _canvasTransform, false);
            _joystick2.OnPointerDown();
            JumpTouching = true;
            _jumpInitialized = true;
        }

        if (_walkInitialized && !LeftTouching)
        {
            _joystick1.StartFading();
            _joystick1.OnPointerUp();
            _stickiness.StopWalking(_jumper.ReadyToJump());
            WalkTouching = false;
            DoubleTouching = false;
            _walkInitialized = false;
        }

        if (_jumpInitialized && !RightTouching)
        {
            _joystick2.StartFading();
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
                            _jumper.Jump(_joystick2.Direction);
                            break;
                        case JumpMode.Charge:
                            _jumper.Charge(_joystick2.Direction);
                            break;
                    }
                }
            }
            JumpTouching = false;
            _stickiness.ReinitSpeed();
            _hero.StopDisplayGhosts();
            DoubleTouching = false;
            _joystick2.OnPointerUp();

            _jumpInitialized = false;
        }

        if (_walkInitialized && LeftTouching)
        {
            _joystick1.Drag(LeftTouch.Value);
        }

        if (_jumpInitialized && RightTouching)
        {
            _joystick2.Drag(RightTouch.Value);
        }
    }

    private void LateUpdate()
    {
        if (!HeroSpawned())
            return;

        if (JumpDragging)
        {
            if (WalkTouching && !DoubleTouching)
            {
                DoubleTouching = true;
            }

            if (_jumper.CanJump())
            {
                if (WalkTouching && !WalkDragging)
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

                _jumper.Trajectory.DrawTrajectory(_hero.transform.position, _joystick2.Direction);
            }
            else if (_jumper.TrajectoryInUse())
            {
                _jumper.ReinitJump();
            }

            if (_jumper.ReadyToJump())
            {
                _stickiness.StopWalking(false);
            }
        }
        else if (WalkDragging && _stickiness.Attached && _stickiness.CanWalk)
        {
            if (JumpTouching && !DoubleTouching)
            {
                _stickiness.CurrentSpeed *= 2;
                _hero.StartDisplayGhosts();

                DoubleTouching = true;
            }

            _stickiness.StartWalking();
        }
        else
        {
            if (_stickiness.Attached)
            {
                _stickiness.Rigidbody.velocity = new Vector2(0, 0);
            }
        }
    }

    private bool HeroSpawned()
    {
        //Waiting for hero to spawn
        if (_hero == null)
        {
            _hero = Hero.Instance;
            _stickiness = _hero?.Stickiness;
            _jumper = _hero?.Jumper as HeroJumper;
            _dynamicInteraction = _hero?.DynamicInteraction;
            if (_hero == null)
                return false;
        }

        return true;
    }

    public Vector3 GetWalkDirection()
    {
        return _joystick1.Direction;
    }

    public Vector3 GetJumpDirection()
    {
        return _joystick2.Direction;
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

            if (touch.position.x > Screen.width / 2)
                return touch.position;
        }

        return null;
    }

    private bool IsTouching (TouchType touchType)
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
