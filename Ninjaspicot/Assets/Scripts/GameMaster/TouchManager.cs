using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private bool _mobileTouch;
    [SerializeField] private Joystick _joystick1;
    [SerializeField] private Joystick _joystick2;

    public bool WalkTouching { get; private set; }
    public bool JumpTouching { get; private set; }
    private TimeManager _timeManager;

    public bool WalkDragging => _joystick1.Direction.magnitude > .5f;
    public bool JumpDragging => _joystick2.Direction.magnitude > .5f;

    public bool DoubleTouching { get; private set; }

    private static TouchManager _instance;
    public static TouchManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TouchManager>(); return _instance; } }

    private Hero _hero;
    private Stickiness _stickiness;
    private HeroJumper _jumper;
    private DynamicInteraction _dynamicInteraction;

    private void Awake()
    {
        _timeManager = TimeManager.Instance;
    }

    private void Start()
    {
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
    }

    private void FixedUpdate()
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
                if (WalkTouching)
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

    public void GetTouchDown(Joystick joystick)
    {
        if (joystick == _joystick1)
        {
            WalkTouching = true;
        }
        else if (joystick == _joystick2)
        {
            JumpTouching = true;
        }
    }

    public void GetTouchUp(Joystick joystick)
    {
        if (joystick == _joystick1)
        {
            WalkTouching = false;
        }
        else if (joystick == _joystick2)
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
                            _jumper.Jump(_joystick2.Direction);
                            break;
                        case JumpMode.Charge:
                            _jumper.Charge(_joystick2.Direction);
                            break;
                    }
                }
            }
            JumpTouching = false;
        }
        _stickiness.ReinitSpeed();
        _hero.StopDisplayGhosts();
        DoubleTouching = false;
    }

    public Vector3 GetWalkDirection()
    {
        return _joystick1.Direction;
    }

    public Vector3 GetJumpDirection()
    {
        return _joystick2.Direction;
    }
}
