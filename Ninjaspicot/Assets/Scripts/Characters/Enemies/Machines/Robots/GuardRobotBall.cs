using System.Collections;
using UnityEngine;

public class GuardRobotBall : RobotBall, IListener
{
    public GuardMode GuardMode { get; private set; }
    public Vector3 TargetPosition { get; private set; }
    public LocationPoint TargetLocation { get; private set; }


    private HearingPerimeter _hearingPerimeter;
    private Quaternion _initRotation;
    private Vector3 _initPos;
    private GuardMode _nextState;
    private float _wonderTime;
    private float _wonderElapsedTime;

    public float Range => 80f;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
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

        switch (GuardMode)
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

    private void StartWondering()
    {
        GuardMode = GuardMode.Wondering;
        Renderer.color = ColorUtils.Red;
        SetReaction(ReactionType.Wonder);
        _wonderElapsedTime = 0;
    }

    private void Wonder()
    {
        _wonderElapsedTime += Time.deltaTime;

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

    private void StartChecking()
    {
        GuardMode = GuardMode.Checking;
        Renderer.color = ColorUtils.Red;
        _nextState = GuardMode.Returning;
        Laser.SetActive(true);
    }

    private void Check()
    {
        var deltaX = TargetPosition.x - Transform.position.x;

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, TargetPosition - Transform.position), Time.deltaTime * _rotateSpeed);

        if (Mathf.Abs(deltaX) > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Time.deltaTime * _moveSpeed);
        }
        else if (Vector2.Dot(Utils.ToVector2(_sprite.right), Utils.ToVector2(TargetPosition - Transform.position).normalized) > .99f)
        {
            _hearingPerimeter.SoundMark?.Deactivate();
            _wonderTime = 5f;
            StartWondering();
        }
    }

    private void StartChasing()
    {
        GuardMode = GuardMode.Chasing;
        Renderer.color = ColorUtils.Red;
        SetReaction(ReactionType.Find);
        Laser.SetActive(true);
    }

    private void Chase(Vector3 target)
    {
        var deltaX = target.x - Transform.position.x;

        if (Mathf.Abs(deltaX) > 2)
        {
            _rigidbody.MovePosition(_rigidbody.position + Vector2.right * Mathf.Sign(deltaX) * Time.deltaTime * _moveSpeed);
        }

        _sprite.rotation = Quaternion.RotateTowards(_sprite.rotation, Quaternion.Euler(0f, 0f, 90f) * Quaternion.LookRotation(Vector3.forward, target - Transform.position), Time.deltaTime * _rotateSpeed);
    }

    private void StartReturning()
    {
        GuardMode = GuardMode.Returning;
        SetReaction(ReactionType.Patrol);
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
    }

    private void Return()
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

    private void StartGuarding()
    {
        GuardMode = GuardMode.Guarding;
        Renderer.color = ColorUtils.White;
        Laser.SetActive(false);
    }

    public void Guard()
    {

    }

    public void Hear(HearingArea hearingArea)
    {
        TargetPosition = hearingArea.GetSource();
        TargetLocation = hearingArea.ClosestLocation;

        if (_reactionType == ReactionType.Sleep)
        {
            _nextState = GuardMode.Checking;
            _wonderTime = 1f;
            StartWondering();
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
        _reaction?.Activate();
        _characterLight.Wake();
    }

    public override void Deactivate()
    {
        Active = false;
        Renderer.color = ColorUtils.White;
        Laser?.Deactivate();
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

    public override void Die(Transform killer = null)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Dying()
    {
        throw new System.NotImplementedException();
    }
}