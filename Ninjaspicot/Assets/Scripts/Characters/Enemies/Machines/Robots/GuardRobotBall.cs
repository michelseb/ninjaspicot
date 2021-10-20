using System;
using UnityEngine;

public class GuardRobotBall : RobotBall, IListener, IViewer
{
    [SerializeField] private float _hearingRange;
    [SerializeField] protected float _checkWonderTime;
    [SerializeField] protected float _returnWonderTime;

    public Vector3 TargetPosition { get; protected set; }
    public Transform TargetTransform { get; protected set; }

    protected HearingPerimeter _hearingPerimeter;
    protected float _wonderTime;
    protected float _wonderElapsedTime;
    protected Quaternion _initRotation;
    private Audio _reactionSound;
    private TimeManager _timeManager;

    public float Range => _hearingRange;
    public bool Seeing { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _hearingPerimeter = GetComponentInChildren<HearingPerimeter>();
        _timeManager = TimeManager.Instance;
    }

    protected override void Start()
    {
        base.Start();
        _reactionSound = _audioManager.FindAudioByName("RobotReact");
        _initRotation = _sprite.rotation;
        Laser.Deactivate();
    }

    protected override void Update()
    {
        base.Update();

        if (!Active)
            return;

        HandleState(_state.StateType);
    }

    protected virtual void HandleState(StateType stateType)
    {
        switch (stateType)
        {
            case StateType.Guard:
            case StateType.Patrol:
                Guard();
                break;

            case StateType.Wonder:
                Wonder();
                break;

            case StateType.Check:
                Check();
                break;

            case StateType.Chase:
                Chase(TargetTransform.position);
                break;

            case StateType.Return:
                Return();
                break;
        }
    }

    protected virtual void StartWondering(StateType nextState)
    {
        if (!IsState(StateType.Chase))
        {
            _audioManager.PlaySound(_audioSource, _reactionSound, .4f);
        }

        SetNextState(nextState);
        _wonderTime = GetReactionFactor(_initState);
        Renderer.color = ColorUtils.Red;
        _wonderElapsedTime = 0;
    }

    protected virtual void Wonder()
    {
        _wonderElapsedTime += Time.deltaTime;

        if (_wonderElapsedTime >= _wonderTime)
        {
            switch (_state.NextState)
            {
                case StateType.Check:
                    SetState(StateType.Check);
                    break;
                case StateType.Chase:
                    SetState(StateType.Chase, TargetTransform);
                    break;
                case StateType.Return:
                    SetState(StateType.Return);
                    break;
            }
        }
    }

    protected virtual void StartChecking()
    {
        SetNextState(StateType.Return);
        Renderer.color = ColorUtils.Red;
    }

    protected virtual void Check()
    {
        var deltaX = TargetPosition.x - Transform.position.x;

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, TargetPosition - Transform.position), Time.deltaTime * _rotateSpeed);

        //var alignedWithTarget = 

        //if (alignedWithTarget && !Laser.Active)
        //{
        //    Laser.SetActive(true);
        //}

        var direction = Vector2.right * Mathf.Sign(deltaX);

        var wallNear = Utils.RayCast(_rigidbody.position, direction, 6, Id);

        if (Mathf.Abs(deltaX) > 2 && !wallNear)
        {
            _rigidbody.MovePosition(_rigidbody.position + direction * Time.deltaTime * _moveSpeed);
        }
        else if (Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(TargetPosition - Transform.position).normalized) > .99f)
        {
            _hearingPerimeter.EraseSoundMark();
            SetState(StateType.Wonder, StateType.Return);
        }
    }

    protected virtual void StartChasing(Transform target)
    {
        TargetTransform = target;
        Renderer.color = ColorUtils.Red;
        Laser.SetActive(true);
    }

    protected virtual void Chase(Vector3 target)
    {
        var hero = Hero.Instance;
        var deltaX = target.x - Transform.position.x;

        var direction = Vector2.right * Mathf.Sign(deltaX);

        var wallNear = Utils.RayCast(_rigidbody.position, direction, 6, Id);
        if (Mathf.Abs(deltaX) > 2 && !wallNear)
        {
            _rigidbody.MovePosition(_rigidbody.position + direction * Time.deltaTime * _moveSpeed);
        }


        var heroNotVisible = Utils.LineCast(Transform.position, target, new int[] { Id, hero.Id });

        if (heroNotVisible)
        {
            Seeing = false;
            SetState(StateType.Wonder, StateType.Return);
        }

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);

        //var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(target - Transform.position).normalized) > .99f;

        //if (alignedWithTarget && !Laser.Active)
        //{
        //    Laser.SetActive(true);
        //}

    }

    protected virtual void StartLookFor()
    {
    }

    protected virtual void LookFor()
    {

    }

    protected virtual void StartReturning()
    {
        TargetTransform = null;
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
    }

    protected virtual void Return()
    {
        var deltaX = _resetPosition.x - Transform.position.x;

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, _initRotation, Time.deltaTime * _rotateSpeed);

        if (deltaX > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Time.deltaTime * _moveSpeed);
        }
        else if (Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(_resetPosition - Transform.position).normalized) > .99f)
        {
            SetState(StateType.Guard);
        }
    }

    protected virtual void StartGuarding()
    {
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
    }

    protected virtual void Guard() { }

    protected virtual void StartSleeping()
    {
        Laser.Deactivate();
        FieldOfView.Deactivate();
    }

    public void Hear(HearingArea hearingArea)
    {
        TargetPosition = hearingArea.SourcePoint;

        if (IsState(StateType.Chase) || (IsState(StateType.Wonder) && IsNextState(StateType.Check)))
            return;

        if (_initState == StateType.Sleep)
        {
            FieldOfView.Activate();
            SetState(StateType.Wonder, StateType.Check);
        }
        else if (!IsState(StateType.Check) && !IsState(StateType.Chase))
        {
            SetState(StateType.Check);
        }
    }

    public void See(Transform target)
    {
        if (Seeing)
            return;

        var hero = Hero.Instance;
        var raycast = Utils.LineCast(Transform.position, hero.Transform.position, new int[] { Id, hero.Id });

        // Visible when walking in the dark ?
        if (!hero.Visible /*&& !hero.Stickiness.Walking*/)
            return;

        if (raycast)
            return;

        Seeing = true;

        // Temps de réaction
        if (TargetTransform == null)
        {
            _timeManager.SlowDown();
            _timeManager.StartTimeRestore();
            TargetTransform = target;
        }

        SetState(StateType.Chase, target);
    }

    public override void Sleep()
    {
        base.Sleep();

        _hearingPerimeter.EraseSoundMark();
        _hearingPerimeter.Deactivate();
    }

    public override void Wake()
    {
        base.Wake();

        _hearingPerimeter.Activate();
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1)
    {
        _hearingPerimeter.EraseSoundMark();
        base.Die(killer, sound, volume);
    }

    protected float GetReactionFactor(StateType reactionType)
    {
        switch (reactionType)
        {
            case StateType.Sleep:
                return 2;
            case StateType.Patrol:
            case StateType.Guard:
                return 1.3f;
            case StateType.Wonder:
                return .5f;
            default:
                return 1;
        }
    }

    public override void DoReset()
    {
        Seeing = false;
        TargetTransform = null;
        base.DoReset();
    }

    protected override Action GetActionFromState(StateType stateType, object parameter = null)
    {
        switch (stateType)
        {
            case StateType.Sleep:
                return StartSleeping;
            case StateType.Wonder:
                return () => StartWondering((StateType)parameter);
            case StateType.Check:
                return StartChecking;
            case StateType.Chase:
                return () => StartChasing((Transform)parameter);
            case StateType.Return:
                return StartReturning;
            case StateType.LookFor:
                return StartLookFor;
            case StateType.Guard:
            case StateType.Patrol:
                return StartGuarding;

            default:
                return null;
        }
    }
}