using System;
using UnityEngine;

public abstract class Robot : Enemy, IListener, IViewer
{
    [SerializeField] protected Transform _head;
    [SerializeField] protected float _hearingRange;
    [SerializeField] protected float _checkWonderTime;
    [SerializeField] protected float _returnWonderTime;
    [SerializeField] protected float _timeBetweenCommunications;
    [SerializeField] protected float _communicationTime;

    protected HearingPerimeter _hearingPerimeter;
    protected Rigidbody2D _rigidbody;
    protected TimeManager _timeManager;
    protected Audio _reactionSound;
    protected Quaternion _initRotation;
    protected float _remainingTimeBeforeCommunication;
    protected float _remainingCommunicationTime;
    protected float _wonderTime;
    protected float _wonderElapsedTime;

    protected FieldOfView _fieldOfView;
    public FieldOfView FieldOfView { get { if (Utils.IsNull(_fieldOfView)) _fieldOfView = GetComponentInChildren<FieldOfView>(); return _fieldOfView; } }
    protected RobotLaser _laser;
    public RobotLaser Laser { get { if (Utils.IsNull(_laser)) _laser = GetComponentInChildren<RobotLaser>(); return _laser; } }
    public Transform TargetTransform { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }

    public float Range => _hearingRange;
    public bool Seeing { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
        _hearingPerimeter = GetComponentInChildren<HearingPerimeter>();
        _timeManager = TimeManager.Instance;
    }

    protected override void Start()
    {
        base.Start();
        _reactionSound = _audioManager.FindAudioByName("RobotReact");
        _initRotation = _head.rotation;

        Laser.Deactivate();
    }

    protected virtual void Update()
    {
        if (!Active)
            return;

        var rot = _head.rotation.eulerAngles.z;
        Renderer.flipY = rot > 90 && rot < 270;

        HandleState(State.StateType);
        HandleCommunications();
    }

    #region Handlers
    protected virtual void HandleCommunications()
    {
        if (!IsState(StateType.Patrol) && !IsState(StateType.Guard))
            return;

        _remainingTimeBeforeCommunication -= Time.deltaTime;

        if (_remainingTimeBeforeCommunication <= 0)
        {
            SetState(StateType.Communicate, State.StateType);
        }
    }

    protected virtual void HandleState(StateType stateType)
    {
        switch (stateType)
        {
            case StateType.Guard:
                Guard();
                break;
            case StateType.Patrol:
                Patrol();
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

            case StateType.LookFor:
                LookFor();
                break;

            case StateType.Communicate:
                Communicate();
                break;
        }
    }
    #endregion

    protected virtual void StartSleeping()
    {
        Laser.Deactivate();
        FieldOfView.Deactivate();
    }

    protected virtual void StartWondering(StateType nextState)
    {
        if (!IsState(StateType.Chase))
        {
            _audioManager.PlaySound(_audioSource, _reactionSound, .4f);
        }

        FieldOfView.Activate();
        SetNextState(nextState);
        _wonderTime = GetReactionFactor(_initState);
        Renderer.color = ColorUtils.Red;
        _wonderElapsedTime = 0;
    }


    protected virtual void StartChecking()
    {
        if (IsNextState(StateType.Check))
        {
            _initRotation = Transform.rotation;
        }

        SetNextState(StateType.Return);
        Renderer.color = ColorUtils.Red;
        FieldOfView.Activate();
    }

    protected virtual void StartChasing()
    {
        if (IsState(StateType.Guard))
        {
            _initRotation = Transform.rotation;
        }

        TargetTransform = Hero.Instance.Transform;
        Renderer.color = ColorUtils.Red;
        Laser.SetActive(true);
        FieldOfView.Activate();
    }

    protected virtual void StartLookFor()
    {
        Renderer.color = ColorUtils.Yellow;
        FieldOfView.Activate();
    }

    protected virtual void StartReturning()
    {
        TargetTransform = null;
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
        FieldOfView.Activate();
    }

    protected virtual void StartCommunicate(StateType nextState)
    {
        FieldOfView.Deactivate();
        _remainingTimeBeforeCommunication = _timeBetweenCommunications;
        _remainingCommunicationTime = _communicationTime;
        SetNextState(nextState);
    }

    protected virtual void StartGuarding()
    {
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
        FieldOfView.Activate();
    }

    protected virtual void StartPatrolling()
    {
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
        FieldOfView.Activate();
    }


    protected virtual void Wonder()
    {
        _wonderElapsedTime += Time.deltaTime;

        if (_wonderElapsedTime >= _wonderTime)
        {
            SetState(State.NextState);
        }
    }

    protected abstract void Guard();
    protected abstract void Patrol(); 
    protected abstract void Check(); 
    protected abstract void Chase(Vector3 targetPosition); 
    protected abstract void Return(); 
    protected abstract void LookFor(); 
    protected abstract void Communicate(); 

    public override void Sleep()
    {
        base.Sleep();

        FieldOfView.Deactivate();
        Laser?.Deactivate();
        _castArea.enabled = false;
        _hearingPerimeter.EraseSoundMark();
        _hearingPerimeter.Deactivate();
    }

    public override void Wake()
    {
        base.Wake();

        _castArea.enabled = true;
        _hearingPerimeter.Activate();

        if (_initState != StateType.Sleep)
        {
            FieldOfView.Activate();
        }
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1f)
    {
        if (Dead)
            return;

        if (killer != null)
        {
            _rigidbody.AddForce((Transform.position - killer.position).normalized * 50, ForceMode2D.Impulse);
        }

        base.Die(killer, sound, volume);
    }

    public override void DoReset()
    {
        StopAllCoroutines();
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        Laser.SetActive(false);
        base.DoReset();
    }


    protected float GetReactionFactor(StateType stateType)
    {
        switch (stateType)
        {
            case StateType.Sleep:
                return 3;
            case StateType.Patrol:
            case StateType.Guard:
                return 1.3f;
            case StateType.Wonder:
                return .5f;
            default:
                return 1;
        }
    }

    protected override Action GetActionFromState(StateType stateType, StateType? nextState = null)
    {
        switch (stateType)
        {
            case StateType.Sleep:
                return StartSleeping;

            case StateType.Wonder:
                return () => StartWondering(nextState.Value);

            case StateType.Check:
                return StartChecking;

            case StateType.Chase:
                return StartChasing;

            case StateType.Return:
                return StartReturning;

            case StateType.LookFor:
                return StartLookFor;

            case StateType.Guard:
            case StateType.Patrol:
                return StartGuarding;

            case StateType.Communicate:
                return () => StartCommunicate(nextState.Value);

            default:
                return null;
        }
    }

    #region Events
    public void Hear(HearingArea hearingArea)
    {
        TargetPosition = hearingArea.SourcePoint;

        if (IsState(StateType.Chase) || (IsState(StateType.Wonder) && IsNextState(StateType.Check)))
            return;

        FieldOfView.Activate();
        if (!IsState(StateType.Check) && !IsState(StateType.Chase))
        {
            SetState(StateType.Wonder, StateType.Check);
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

        SetState(StateType.Chase);
    }
    #endregion
}