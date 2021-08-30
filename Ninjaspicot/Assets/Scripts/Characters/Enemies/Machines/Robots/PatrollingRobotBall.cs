using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrollingRobotBall : GuardRobotBall
{
    [SerializeField] TargetPoint _target;
    [SerializeField] TargetPoint[] _targetPoints;
    protected IList<Vector3> _pathPositions;

    private Vector3 _currentPathTarget;
    private Vector3? _targetPosition;

    protected override void Awake()
    {
        base.Awake();
        _pathPositions = _targetPoints.Where(p => !p.IsAim).Select(x => x.transform.position).ToList();
        _targetPosition = _target?.transform.position;
        InitPathTarget();
    }
    protected override void Start()
    {
        if (_targetPosition.HasValue)
        {
            _sprite.rotation = Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
        }

        base.Start();

        Laser.SetActive(true);
    }

    protected override void Guard()
    {
        if (_targetPosition.HasValue)
        {
            _sprite.rotation = Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
        }

        var direction = Utils.ToVector2(_currentPathTarget - Transform.position);

        _rigidbody.MovePosition(_rigidbody.position + direction.normalized * Time.deltaTime * _moveSpeed);

        if (direction.magnitude < 1)
        {
            SetNextPathTarget();
        }
    }

    protected override void Check()
    {
        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, TargetPosition - Transform.position), Time.deltaTime * _rotateSpeed);

        if (Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(TargetPosition - Transform.position).normalized) > .99f)
        {
            _hearingPerimeter.SoundMark?.Deactivate();
            StartWondering(GuardMode.Returning, 3f);
        }

        if (Hero.Instance.Dead)
        {
            StartWondering(GuardMode.Returning, 1f);
        }
    }

    protected override void Chase(Vector3 target)
    {
        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);

        if (Hero.Instance.Dead)
        {
            StartWondering(GuardMode.Returning, 1f);
        }
    }

    protected override void Return()
    {
        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, _initRotation, Time.deltaTime * _rotateSpeed);

        if (Quaternion.Angle(_initRotation, _sprite.rotation) < .1f)
        {
            StartGuarding();
        }
    }

    protected override void StartReturning()
    {
        GuardMode = GuardMode.Returning;
        SetReaction(ReactionType.Patrol);
        Renderer.color = ColorUtils.White;
    }
    protected override void StartGuarding()
    {
        GuardMode = GuardMode.Guarding;
        Renderer.color = ColorUtils.White;
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