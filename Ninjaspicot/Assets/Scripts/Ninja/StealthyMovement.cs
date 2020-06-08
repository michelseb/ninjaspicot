using System.Collections;
using UnityEngine;

public enum Dir
{
    Left,
    Right
}

public class StealthyMovement : Movement // Movement that includes walking on walls
{
    [SerializeField] private int _maxJumps;
    public bool Active { get; set; }

    private Coroutine _walkOnWalls;
    private TouchManager _touchManager;
    private Stickiness _stickiness;
    private PoolManager _poolManager;
    protected Trajectory _trajectory;

    private Dir _ninjaDir;

    protected override void Awake()
    {
        base.Awake();
        _stickiness = GetComponent<Stickiness>();
        _poolManager = PoolManager.Instance;
    }

    protected override void Start()
    {
        base.Start();

        SetMaxJumps(_maxJumps);
        GainAllJumps();
        _strength = 100;
        _touchManager = TouchManager.Instance;
        _cameraBehaviour.Zoom(ZoomType.Intro);
    }

    protected void Update()
    {
        if (!Active)
            return;

        if (_touchManager.Touching)
        {
            _ninjaDir = _touchManager.TouchArea == TouchArea.Left ? Dir.Left : Dir.Right;

            if (CanJump())
            {
                StopWalking();

                _trajectory = GetTrajectory();
                _trajectory.DrawTrajectory(transform.position, _touchManager.TouchDrag, _touchManager.RawTouchOrigin, _strength);
            }
            else
            {
                if (_walkOnWalls == null && _stickiness.Attached)
                {
                    _walkOnWalls = StartCoroutine(WalkOnWalls(_stickiness.WallJoint));
                }

                //_trajectory.ReinitTrajectory();
                //_timeManager.SetNormalTime();
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            //_touchManager.Erase();

            if (CanJump())
            {
                if (!_trajectory.IsClear(transform.position, 2))//Add ninja to new layer
                {
                    _trajectory.ReinitTrajectory();
                    _timeManager.SetNormalTime();
                }
                else
                {
                    StopWalking();
                    Jump(_touchManager.RawTouchOrigin, _touchManager.TouchDrag, _strength);
                }
                _touchManager.ReinitDrag();
            }
        }
    }

    private void FixedUpdate()
    {
        if (_stickiness.Attached && !_touchManager.Touching)
        {
            _dynamicEntity.Rigidbody.velocity = new Vector2(0, 0);
        }
    }

    public override void Jump(Vector2 origin, Vector2 drag, float strength)
    {
        _stickiness.Detach();
        base.Jump(origin, drag, strength);
        _poolManager.GetPoolable<Dash>(transform.position, Quaternion.LookRotation(Vector3.forward, drag - origin));
        _trajectory.ReinitTrajectory();
        _trajectory = null;
        _cameraBehaviour.DoShake(.3f, .1f);
    }


    private Trajectory GetTrajectory()
    {
        if (_trajectory == null)
        {
            return _poolManager.GetPoolable<Trajectory>(transform.position, Quaternion.identity);
        }

        return _trajectory;
    }

    public void StopWalking()
    {
        if (_walkOnWalls != null)
        {
            StopCoroutine(_walkOnWalls);
        }
        _stickiness.WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

    public IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;
        _stickiness.WallJoint.useMotor = true;

        while (Input.GetButton("Fire1"))
        {

            if (_ninjaDir == Dir.Right)
            {
                jointMotor.motorSpeed = _currentSpeed;
            }
            else if (_ninjaDir == Dir.Left)
            {
                jointMotor.motorSpeed = -_currentSpeed;
            }
            hinge.motor = jointMotor;
            hinge.anchor = transform.InverseTransformPoint(_stickiness.GetContactPosition());
            hinge.connectedAnchor = hinge.anchor;

            yield return null;
        }


        _stickiness.WallJoint.useMotor = false;
        _walkOnWalls = null;
    }

    private bool CanJump()
    {
        return GetJumps() > 0 && _touchManager.Dragging;
    }

}
