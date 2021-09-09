using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrollingRobotBall : GuardRobotBall
{
    [SerializeField] TargetPoint _target;
    [SerializeField] TargetPoint[] _targetPoints;
    protected IList<Vector3> _pathPositions;

    private Vector3? _currentPathTarget;
    private Vector3? _targetPosition;

    protected override void Awake()
    {
        base.Awake();
        _pathPositions = _targetPoints != null ? _targetPoints.Where(p => p != null && !p.IsAim).Select(x => x.transform.position).ToList() : new List<Vector3>();
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
        FieldOfView.Activate();
        //Laser.SetActive(true);
    }

    protected override void Guard()
    {
        if (_targetPosition.HasValue)
        {
            _sprite.rotation = Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
        }

        if (!_currentPathTarget.HasValue)
            return;

        var direction = Utils.ToVector2(_currentPathTarget.Value - Transform.position);

        _rigidbody.MovePosition(_rigidbody.position + direction.normalized * Time.deltaTime * _moveSpeed);

        if (direction.magnitude < 1)
        {
            SetNextPathTarget();
        }
    }

    protected override void StartWondering(GuardMode nextState, float wonderTime)
    {
        if (nextState == GuardMode.Checking)
        {
            _initRotation = Transform.rotation;
        }

        base.StartWondering(nextState, wonderTime);
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

    protected override void StartChasing(Transform target)
    {
        if (GuardMode == GuardMode.Guarding)
        {
            _initRotation = Transform.rotation;
        }

        base.StartChasing(target);
    }

    protected override void Chase(Vector3 target)
    {
        var hero = Hero.Instance;

        var raycast = Utils.LineCast(Transform.position, target, new int[] { Id, hero.Id });

        if (raycast)
        {
            StartWondering(GuardMode.Returning, 3f);
        }

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);

        var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(target - Transform.position).normalized) > .99f;

        if (alignedWithTarget && !Laser.Active)
        {
            Laser.SetActive(true);
        }

        if (hero.Dead)
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
        Laser.SetActive(false);
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
        if (_pathPositions == null || _pathPositions.Count == 0)
            return;

        _pathPositions.RemoveAt(0);
        _pathPositions.Add(_currentPathTarget.Value);
        _currentPathTarget = _pathPositions[0];
    }
}