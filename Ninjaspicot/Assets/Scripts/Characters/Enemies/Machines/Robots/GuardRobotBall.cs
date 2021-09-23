using UnityEngine;

public class GuardRobotBall : RobotBall, IListener, IViewer
{
    [SerializeField] private float _hearingRange;

    public GuardMode GuardMode { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }
    public Transform TargetTransform { get; protected set; }

    protected HearingPerimeter _hearingPerimeter;
    protected GuardMode _nextState;
    protected float _wonderTime;
    protected float _wonderElapsedTime;
    protected Quaternion _initRotation;
    private Vector3 _initPos;
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
        _initPos = Transform.position;
        _initRotation = _sprite.rotation;
        GuardMode = GuardMode.Guarding;
        Laser.SetActive(false);
        Activate();
    }

    protected override void Update()
    {
        base.Update();

        if (!Active)
            return;

        HandleState(GuardMode);
    }

    protected virtual void HandleState(GuardMode mode)
    {
        switch (mode)
        {
            case GuardMode.Guarding:
                Guard();
                break;

            case GuardMode.Wondering:
                Wonder();
                break;

            case GuardMode.Checking:
                Check();
                break;

            case GuardMode.Chasing:
                Chase(TargetTransform.position);
                break;

            case GuardMode.Returning:
                Return();
                break;
        }
    }

    protected virtual void StartWondering(GuardMode nextState, float wonderTime)
    {
        if (GuardMode == GuardMode.Guarding || GuardMode == GuardMode.Returning)
        {
            _audioManager.PlaySound(_audioSource, _reactionSound, .4f);
        }

        GuardMode = GuardMode.Wondering;
        _wonderTime = wonderTime;
        _nextState = nextState;
        Renderer.color = ColorUtils.Red;
        SetReaction(ReactionType.Wonder);
        _wonderElapsedTime = 0;
    }

    protected virtual void Wonder()
    {
        _wonderElapsedTime += Time.deltaTime;

        if (_nextState == GuardMode.Returning && Hero.Instance.Dead && _wonderTime > 1f)
        {
            StartWondering(GuardMode.Returning, 1f);
        }

        if (_wonderElapsedTime >= _wonderTime)
        {
            //_reaction?.Deactivate();

            switch (_nextState)
            {
                case GuardMode.Checking:
                    StartChecking();
                    break;
                case GuardMode.Chasing:
                    StartChasing(TargetTransform);
                    break;
                case GuardMode.Returning:
                    StartReturning();
                    break;
            }
        }
    }

    protected virtual void StartChecking()
    {
        GuardMode = GuardMode.Checking;
        Renderer.color = ColorUtils.Red;
        _nextState = GuardMode.Returning;
        //Laser.SetActive(true);
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
            _hearingPerimeter.SoundMark?.Deactivate();
            StartWondering(GuardMode.Returning, 3f);
        }

        if (Hero.Instance.Dead)
        {
            StartWondering(GuardMode.Returning, 1f);
        }
    }

    protected virtual void StartChasing(Transform target)
    {
        TargetTransform = target;
        GuardMode = GuardMode.Chasing;
        Renderer.color = ColorUtils.Red;
        SetReaction(ReactionType.Find);
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
            StartWondering(GuardMode.Returning, 3f);
        }

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);

        //var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(target - Transform.position).normalized) > .99f;

        //if (alignedWithTarget && !Laser.Active)
        //{
        //    Laser.SetActive(true);
        //}

        if (hero.Dead)
        {
            Seeing = false;
            StartWondering(GuardMode.Returning, 1f);
        }

    }

    protected virtual void StartReturning()
    {
        TargetTransform = null;
        GuardMode = GuardMode.Returning;
        SetReaction(ReactionType.Patrol);
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
    }

    protected virtual void Return()
    {
        var deltaX = _initPos.x - Transform.position.x;

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, _initRotation, Time.deltaTime * _rotateSpeed);

        if (deltaX > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Time.deltaTime * _moveSpeed);
        }
        else if (Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(_initPos - Transform.position).normalized) > .99f)
        {
            StartGuarding();
        }
    }

    protected virtual void StartGuarding()
    {
        GuardMode = GuardMode.Guarding;
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
    }

    protected virtual void Guard() { }

    public void Hear(HearingArea hearingArea)
    {
        TargetPosition = hearingArea.SourcePoint;

        if (GuardMode == GuardMode.Chasing || (GuardMode == GuardMode.Wondering && _nextState == GuardMode.Checking))
            return;

        if (_reactionType == ReactionType.Sleep)
        {
            FieldOfView.Activate();
            StartWondering(GuardMode.Checking, 2f);
        }
        else if (GuardMode != GuardMode.Checking && GuardMode != GuardMode.Chasing)
        {
            StartChecking();
        }
    }

    public void See(Transform target)
    {
        if (Seeing)
            return;

        var hero = Hero.Instance;
        var raycast = Utils.LineCast(Transform.position, hero.Transform.position, new int[] { Id, hero.Id });

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

        StartChasing(target);
    }

    public override void Activate()
    {
        Active = true;
        Laser?.Activate();
        _hearingPerimeter.Activate();
        _reaction?.Activate();
        _characterLight.Wake();

        if (_reactionType != ReactionType.Sleep)
        {
            FieldOfView.Activate();
        }
    }

    public override void Deactivate()
    {
        Active = false;
        Renderer.color = ColorUtils.White;
        Laser?.Deactivate();
        _hearingPerimeter.Deactivate();
        _reaction?.Deactivate();
        _characterLight.Sleep();
        FieldOfView.Deactivate();
    }

    public override void Sleep()
    {
        Deactivate();
    }

    public override void Wake()
    {
        Activate();
    }
}