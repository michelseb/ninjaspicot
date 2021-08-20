using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrollingRobotBall : RobotBall
{
    protected Vector3? _targetPosition;
    protected IList<Vector3> _pathPositions;

    private Vector3 _currentPathTarget;

    protected override void Awake()
    {
        base.Awake();
        var positions = transform.parent.GetComponentsInChildren<TargetPoint>();
        _targetPosition = positions.FirstOrDefault(p => p.IsAim)?.transform.position;
        _pathPositions = positions.Where(p => !p.IsAim).Select(x => x.transform.position).ToList();
        InitPathTarget();
    }
    protected override void Start()
    {
        Laser.SetActive(true);
        _sprite.rotation = Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (!Active)
            return;

        if (_targetPosition != null)
        {
            _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position), Time.deltaTime * _rotateSpeed);
        }

        var direction = Utils.ToVector2(_currentPathTarget - Transform.position);

        _rigidbody.MovePosition(_rigidbody.position + direction.normalized * Time.deltaTime * _moveSpeed);

        if (direction.magnitude < 1)
        {
            SetNextPathTarget();
        }
    }

    private void InitPathTarget()
    {
        float previousDelta = float.MaxValue;
        Vector3? previousPos = null;
        foreach (var position in _pathPositions)
        {
            var magnitude = (position - Transform.position).magnitude;
            if (magnitude < previousDelta)
            {
                previousDelta = magnitude;
                previousPos = position;
            }
        }

        if (!previousPos.HasValue)
            return;

        foreach (var position in _pathPositions.ToArray())
        {
            if (position == previousPos.Value)
                break;

            _pathPositions.RemoveAt(0);
            _pathPositions.Add(position);
        }


        _currentPathTarget = previousPos.Value;
    }

    private void SetNextPathTarget()
    {
        _pathPositions.RemoveAt(0);
        _pathPositions.Add(_currentPathTarget);
        _currentPathTarget = _pathPositions[0];
    }
}