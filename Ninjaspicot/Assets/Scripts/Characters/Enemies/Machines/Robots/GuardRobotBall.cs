using System.Collections;
using UnityEngine;

public class GuardRobotBall : RobotBall, IListener
{
    public GuardMode GuardMode { get; protected set; }
    public Vector3 TargetPosition { get; protected set; }
    public LocationPoint TargetLocation { get; protected set; }


    protected HearingPerimeter _hearingPerimeter;
    protected GuardMode _nextState;
    protected float _wonderTime;
    protected float _wonderElapsedTime;
    protected Quaternion _initRotation;
    private Vector3 _initPos;

    public float Range => 80f;

    protected override void Awake()
    {
        base.Awake();
        _hearingPerimeter = GetComponentInChildren<HearingPerimeter>();
    }

    protected override void Start()
    {
        base.Start();
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
                Chase(Hero.Instance.Transform.position);
                break;

            case GuardMode.Returning:
                Return();
                break;
        }
    }

    protected void StartWondering(GuardMode nextState, float wonderTime)
    {
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

        var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(TargetPosition - Transform.position).normalized) > .99f;

        if (alignedWithTarget && !Laser.Active)
        {
            Laser.SetActive(true);
        }

        if (Mathf.Abs(deltaX) > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Time.deltaTime * _moveSpeed);
        }
        else if (alignedWithTarget)
        {
            _hearingPerimeter.SoundMark?.Deactivate();
            StartWondering(GuardMode.Returning, 3f);
        }

        if (Hero.Instance.Dead)
        {
            StartWondering(GuardMode.Returning, 1f);
        }
    }

    protected virtual void StartChasing()
    {
        GuardMode = GuardMode.Chasing;
        Renderer.color = ColorUtils.Red;
        SetReaction(ReactionType.Find);
        //Laser.SetActive(true);
    }

    protected virtual void Chase(Vector3 target)
    {
        var deltaX = target.x - Transform.position.x;

        if (Mathf.Abs(deltaX) > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Time.deltaTime * _moveSpeed);
        }

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);

        var alignedWithTarget = Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(target - Transform.position).normalized) > .99f;

        if (alignedWithTarget && !Laser.Active)
        {
            Laser.SetActive(true);
        }

        if (Hero.Instance.Dead)
        {
            StartWondering(GuardMode.Returning, 1f);
        }

    }

    protected virtual void StartReturning()
    {
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
        TargetPosition = hearingArea.GetSource();
        TargetLocation = hearingArea.ClosestLocation;

        if (_reactionType == ReactionType.Sleep)
        {
            StartWondering(GuardMode.Checking, 1f);
        }
        else if (GuardMode == GuardMode.Guarding)
        {
            StartChecking();
        }
        else
        {
            StartChasing();
        }
    }

    public override void Activate()
    {
        Active = true;
        Laser?.Activate();
        _hearingPerimeter.Activate();
        _reaction?.Activate();
        _characterLight.Wake();
    }

    public override void Deactivate()
    {
        Active = false;
        Renderer.color = ColorUtils.White;
        Laser?.Deactivate();
        _hearingPerimeter.Deactivate();
        _reaction?.Deactivate();
        _characterLight.Sleep();
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