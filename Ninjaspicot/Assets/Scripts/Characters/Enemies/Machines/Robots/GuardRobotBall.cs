using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuardRobotBall : Robot
{
    [SerializeField] TargetPoint _target;
    [SerializeField] TargetPoint[] _targetPoints;

    protected Coroutine _lookAt;
   

    protected List<TargetPoint> _initialPathTargets;
    protected List<TargetPoint> _pathTargets;

    private TargetPoint _currentPathTarget;
    private Vector3? _targetPosition;
    private Coroutine _waitAtTarget;

    protected override void Awake()
    {
        base.Awake();

        _pathTargets = _targetPoints != null ? _targetPoints.Where(p => p != null && !p.IsAim).ToList() : new List<TargetPoint>();
        _initialPathTargets = _pathTargets;
        _targetPosition = _target?.transform.position;
        InitPathTarget();
    }

    protected override void Start()
    {
        base.Start();
        _initRotation = _head.rotation;
        //_remainingTimeBeforeCommunication = _timeBetweenCommunications;

        if (_targetPosition.HasValue)
        {
            _head.rotation = Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
        }
        else if (_currentPathTarget != null)
        {
            _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _currentPathTarget.transform.position - Transform.position), RotateSpeed);
        }
    }

    #region Check

    protected override void Check()
    {
        var deltaX = TargetPosition.x - Transform.position.x;

        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, TargetPosition - Transform.position), RotateSpeed);

        //var alignedWithTarget = 

        //if (alignedWithTarget && !Laser.Active)
        //{
        //    Laser.SetActive(true);
        //}

        var direction = Vector2.right * Mathf.Sign(deltaX);

        var wallNear = Utils.RayCast(_rigidbody.position, direction, 6, Id);

        if (Mathf.Abs(deltaX) > 2 && !wallNear)
        {
            _rigidbody.MovePosition(_rigidbody.position + direction * Velocity);
        }
        else if (Vector2.Dot(Utils.ToVector2(_head.right), Utils.ToVector2(TargetPosition - Transform.position).normalized) > .99f)
        {
            _hearingPerimeter.EraseSoundMark();
            SetState(StateType.Wonder, StateType.Return);
        }
    }
    #endregion

    #region Chase
    protected override void Chase(Vector3 target)
    {
        var hero = Hero.Instance;
        var deltaX = target.x - Transform.position.x;

        var direction = Vector2.right * Mathf.Sign(deltaX);

        var wallNear = Utils.RayCast(_rigidbody.position, direction, 6, Id);
        if (Mathf.Abs(deltaX) > 2 && !wallNear)
        {
            _rigidbody.MovePosition(_rigidbody.position + direction * Velocity);
        }


        var heroNotVisible = Utils.LineCast(Transform.position, target, new int[] { Id, hero.Id });

        if (heroNotVisible)
        {
            Seeing = false;
            SetState(StateType.Wonder, StateType.Return);
        }

        _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), RotateSpeed);

        //var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(target - Transform.position).normalized) > .99f;

        //if (alignedWithTarget && !Laser.Active)
        //{
        //    Laser.SetActive(true);
        //}

    }
    #endregion

    #region LookFor

    protected override void LookFor()
    {
        if (_lookAt == null)
        {
            _lookAt = StartCoroutine(LookAtRandom());
        }
        Guard();
    }

    protected virtual IEnumerator LookAtRandom()
    {
        float elapsedTime = 0;
        float delay = 2;
        var direction = UnityEngine.Random.insideUnitCircle.normalized;

        while (elapsedTime < delay)
        {
            elapsedTime += Time.deltaTime;
            _head.rotation = Quaternion.RotateTowards(_head.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, Transform.TransformDirection(direction)), RotateSpeed);
            yield return null;
        }

        _lookAt = null;
    }
    #endregion

    //#region Communicate
    //protected override void Communicate()
    //{
    //    _remainingCommunicationTime -= Time.deltaTime;

    //    if (_remainingCommunicationTime <= 0)
    //    {
    //        if (Zone.DeathOccured)
    //        {
    //            Zone.ActivateAlarm();
    //        }
    //        else
    //        {
    //            SetState(State.NextState);
    //        }
    //    }
    //}
    //#endregion

    #region Return
    protected override void Return()
    {
        var deltaX = _resetPosition.x - Transform.position.x;

        _head.rotation = Quaternion.RotateTowards(_head.rotation, _initRotation, RotateSpeed);

        if (deltaX > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Velocity);
        }
        else if (Vector2.Dot(Utils.ToVector2(_head.right), Utils.ToVector2(_resetPosition - Transform.position).normalized) > .99f)
        {
            SetState(StateType.Guard);
        }
    }
    #endregion

    #region Guard
    protected override void Guard() { }
    #endregion

    #region Guard

    protected override void Patrol() 
    {
        if (_waitAtTarget != null)
            return;

        if (_lookAt == null)
        {
            if (_targetPosition.HasValue)
            {
                _head.rotation = Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _targetPosition.Value - Transform.position);
            }
            else
            {
                Transform.rotation = Quaternion.RotateTowards(Transform.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, _currentPathTarget.transform.position - Transform.position), RotateSpeed);
            }
        }

        if (_currentPathTarget == null)
            return;

        var direction = Utils.ToVector2(_currentPathTarget.transform.position - Transform.position);

        if (_targetPosition.HasValue)
        {
            _rigidbody.MovePosition(_rigidbody.position + direction.normalized * Velocity);
        }
        else
        {
            _rigidbody.MovePosition(_rigidbody.position + Utils.ToVector2(Transform.right) * Velocity);
        }


        if (direction.magnitude < 1)
        {
            UpdateTarget(_currentPathTarget);
        }
    }
    #endregion

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1)
    {
        _hearingPerimeter.EraseSoundMark();
        base.Die(killer, sound, volume);
    }

    public override void DoReset()
    {
        Seeing = false;
        TargetTransform = null;

        if (_lookAt != null)
        {
            StopCoroutine(_lookAt);
            _lookAt = null;
        }

        _pathTargets = _initialPathTargets;

        base.DoReset();
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