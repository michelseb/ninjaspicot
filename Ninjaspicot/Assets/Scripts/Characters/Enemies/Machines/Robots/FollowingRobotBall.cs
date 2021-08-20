using System.Collections.Generic;
using UnityEngine;

public class FollowingRobotBall : RobotBall
{
    [SerializeField] private GuardNinja _master;
    [SerializeField] private float _followSpeed;

    // Four points around the guard
    private Vector2[] _offsets;
    private Vector3 _currentOffset;
    private Rigidbody2D _rigidBody;
    private List<Vector3> _followPositions;
    private Vector3 _targetPosition;

    private const float DIST_BETWEEN_FOLLOW_POSITIONS = 100f;

    protected override void Awake()
    {
        base.Awake();
        _followPositions = new List<Vector3>();
        _rigidBody = GetComponent<Rigidbody2D>();
        Vector2 offset = Transform.position - _master.Transform.position;
        _offsets = new Vector2[] { offset, new Vector2(offset.x, -offset.y), new Vector2(-offset.x, offset.y), -offset };
        _currentOffset = _offsets[0];
        _targetPosition = _master.Transform.position + _currentOffset;
    }

    protected override void Update()
    {
        //base.Update();

        if (!Active)
            return;

        SetAvailableOffset();
        _targetPosition = _master.Transform.position + _currentOffset;

        var normalized = (_targetPosition - Transform.position).normalized;

        var dir = IntermediarySteps() ?
            normalized :
            Vector3.Min(_targetPosition - Transform.position, normalized);

        Renderer.transform.right = _master.Target - Transform.position;
        _rigidBody.velocity = dir * _followSpeed;


        if ((_targetPosition - Transform.position).magnitude < 2)
        {
            SetNextFollowPosition();
        }
    }

    private void SetNextFollowPosition()
    {
        if (!IntermediarySteps())
            return;

        _followPositions.RemoveAt(0);
        _targetPosition = /*IntermediarySteps() ? _followPositions[0] :*/ _master.Transform.position + _currentOffset;
    }

    public void AddFollowPosition(Vector3 position)
    {
        //if (!SetAvailableOffset())
        //    return;

        //position += _currentOffset;

        //var previousPos = IntermediarySteps() ? _followPositions[_followPositions.Count - 1] : Transform.position;
        //if (Vector3.SqrMagnitude(position - previousPos) < DIST_BETWEEN_FOLLOW_POSITIONS)
        //    return;


        //_followPositions.Add(position);

        ////Debug
        ////var sp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ////sp.transform.position = position;

        //if (_followPositions.Count == 1)
        //{
        //    _targetPosition = _followPositions[0];
        //}
    }

    private bool IntermediarySteps()
    {
        return _followPositions.Count > 0;
    }

    private bool SetAvailableOffset()
    {
        Vector2 masterPos = _master.Transform.position;
        for (int i = 0; i < 4; i++)
        {
            var lineCast = Utils.LineCast(_master.Transform.position, masterPos + _offsets[i], new int[] { _master.Id, Id });
            if (!lineCast)
            {
                Debug.Log(i);
                _currentOffset = _offsets[i];
                return true;
            }
            else
            {
                Debug.Log(lineCast.collider.name);
            }
        }

        return false;
    }
}