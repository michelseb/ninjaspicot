using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrollingRobotBall : GuardRobotBall
{
    [SerializeField] TargetPoint _target;
    [SerializeField] TargetPoint[] _targetPoints;
    protected IList<TargetPoint> _pathTargets;

    private TargetPoint _currentPathTarget;
    private Vector3? _targetPosition;
    private Coroutine _waitAtTarget;

    protected override void Awake()
    {
        base.Awake();
        _pathTargets = _targetPoints != null ? _targetPoints.Where(p => p != null && !p.IsAim).ToList() : new List<TargetPoint>();
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
    }

    protected override void Guard()
    {
        if (_waitAtTarget != null)
            return;

        if (_targetPosition.HasValue)
        {
            _sprite.rotation = Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
        }

        if (_currentPathTarget == null)
            return;

        var direction = Utils.ToVector2(_currentPathTarget.transform.position - Transform.position);

        _rigidbody.MovePosition(_rigidbody.position + direction.normalized * Time.deltaTime * _moveSpeed);

        if (direction.magnitude < 1)
        {
            UpdateTarget(_currentPathTarget);
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
            _hearingPerimeter.SoundMark?.Sleep();
            StartWondering(GuardMode.Returning, _returnWonderTime);
        }

        if (Hero.Instance.Dead)
        {
            StartWondering(GuardMode.Returning, _returnWonderTime);
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
            Seeing = false;
            StartWondering(GuardMode.Returning, _returnWonderTime);
        }

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);

        var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(target - Transform.position).normalized) > .99f;

        if (alignedWithTarget && !Laser.Active)
        {
            Laser.SetActive(true);
        }

        if (hero.Dead)
        {
            StartWondering(GuardMode.Returning, _returnWonderTime);
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
        TargetTransform = null;
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
        TargetPoint previousTarget = null;

        foreach (var target in _pathTargets)
        {
            var magnitude = (target.transform.position - Transform.position).magnitude;
            if (magnitude < previousDelta)
            {
                previousDelta = magnitude;
                previousTarget = target;
            }
        }

        if (previousTarget == null)
            return;

        foreach (var targets in _pathTargets.ToArray())
        {
            if (targets == previousTarget)
                break;

            _pathTargets.RemoveAt(0);
            _pathTargets.Add(targets);
        }


        _currentPathTarget = previousTarget;
    }

    private void UpdateTarget(TargetPoint targetPoint)
    {
        if (targetPoint.PauseAmount == 0)
        {
            SetNextPathTarget();
        }
        else
        {
            _waitAtTarget = StartCoroutine(WaitAtTarget(targetPoint.PauseAmount));
        }
    }

    private void SetNextPathTarget()
    {
        if (_pathTargets == null || _pathTargets.Count == 0)
            return;

        _pathTargets.RemoveAt(0);
        _pathTargets.Add(_currentPathTarget);
        _currentPathTarget = _pathTargets[0];
    }

    private IEnumerator WaitAtTarget(float time)
    {
        yield return new WaitForSeconds(time);

        _waitAtTarget = null;
        SetNextPathTarget();
    }
}