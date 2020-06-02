using System.Collections;
using UnityEngine;

public enum Dir
{
    Left,
    Right
}

public class StealthyMovement : Movement // Movement that includes walking on walls
{
    [SerializeField]
    private int _maxJumps;
    [SerializeField]
    private GameObject _dashParticles;

    public bool Started { get; set; }

    private Coroutine _walkOnWalls;
    private TouchManager _touchManager;
    private Stickiness _stickiness;

    private ScenesManager _scenesManager;

    private Dir _ninjaDir;

    protected override void Awake()
    {
        base.Awake();
        _stickiness = GetComponent<Stickiness>();
        _scenesManager = ScenesManager.Instance;
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

        if (!Started)
            return;

        //Debug.Log(n.contact.point.x + " " + n.contact.point.y);
        if (_touchManager.Touching)
        {
            _ninjaDir = _touchManager.RawTouchOrigin.x < Screen.width / 2 ? Dir.Left : Dir.Right;
            _trajectory.Reset();

            if (CanJump())
            {
                //StopWalking();
                //t.Reduce();
                _trajectory.DrawTrajectory(transform.position, _touchManager.TouchDrag, _touchManager.RawTouchOrigin, _strength);
            }
            else
            {
                if (_walkOnWalls == null && _stickiness.Attached)
                {
                    _walkOnWalls = StartCoroutine(WalkOnWalls(_stickiness.WallJoint));
                }

                _trajectory.ReinitTrajectory();
                _timeManager.SetNormalTime();
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
        Instantiate(_dashParticles, transform.position, Quaternion.LookRotation(Vector3.forward, drag - origin));
        _cameraBehaviour.DoShake(.3f, .1f);
    }


    private void StopWalking()
    {
        if (_walkOnWalls != null)
        {
            StopCoroutine(_walkOnWalls);
        }

        _walkOnWalls = null;
    }

    public IEnumerator WalkOnWalls(HingeJoint2D hinge)
    {
        if (hinge == null)
            yield break;

        var jointMotor = hinge.motor;

        while (Input.GetButton("Fire1") && hinge != null)
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

        if (hinge != null)
        {
            jointMotor.motorSpeed = 0;
            hinge.motor = jointMotor;
            hinge.anchor = transform.InverseTransformPoint(_stickiness.GetContactPosition());
            hinge.connectedAnchor = hinge.anchor;
        }

        _walkOnWalls = null;
    }


    //void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(1, 0, 0, .6f);
    //    if (hinge != null)
    //    {
    //        Gizmos.DrawSphere(transform.TransformPoint(hinge.anchor), 1f);
    //    }
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawSphere(n.contact.point, .5f);
    //    foreach (ContactPoint2D c in n.contacts)
    //    {
    //        Gizmos.DrawSphere(c.point, .5f);
    //    }
    //}

    private bool CanJump()
    {
        return GetJumps() > 0 && _touchManager.Dragging;
    }

}
