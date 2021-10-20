using UnityEngine;


public class GuardNinja : EnemyNinja, IListener
{
    //[SerializeField] private FollowingRobotBall _robot;

    public Vector3 Target { get; private set; }

    private float _wonderTime;
    private Vector3 _initPos;
    private GuardStickiness _guardStickiness;
    private EnemyJumper _enemyJumper;
    private HearingPerimeter _hearingPerimeter;

    private float _wonderElapsedTime;


    public float Range => 100f;

    protected override void Awake()
    {
        base.Awake();
        _guardStickiness = Stickiness as GuardStickiness;
        _enemyJumper = Jumper as EnemyJumper;
        _hearingPerimeter = GetComponentInChildren<HearingPerimeter>();
    }

    protected override void Start()
    {
        base.Start();
        _initPos = Transform.position;
        SetState(_initState, GetActionFromState(_initState));
    }

    protected virtual void Update()
    {
        switch (_state.StateType)
        {
            case StateType.Wonder:
                Wonder();
                break;

            case StateType.Check:
                Check();
                break;

            case StateType.Chase:
                Chase(Target);
                break;

            case StateType.Return:
                Return();
                break;

            default:
                break;
        }
    }

    private void StartWondering()
    {
        _wonderElapsedTime = 0;
        //_robot.Activate();
    }

    private void Wonder()
    {
        _wonderElapsedTime += Time.deltaTime;

        if (_wonderElapsedTime >= _wonderTime)
        {
            switch (_state.NextState)
            {
                case StateType.Check:
                    SetState(StateType.Check);
                    break;
                case StateType.Return:
                    SetState(StateType.Return);
                    break;
            }
        }
    }

    private void StartChecking()
    {
        Attacking = true;
        SetNextState(StateType.Return);

        _guardStickiness.StartWalkingTowards(Target);
    }

    private void Check()
    {
        if (Vector2.Distance(Transform.position, Target) < 5)
        {
            _hearingPerimeter.EraseSoundMark();
            _guardStickiness.StopWalkingTowards(false);
            _wonderTime = 5f;
            SetState(StateType.Wonder);
        }
    }

    private void StartChasing()
    {
        Attacking = true;
        _enemyJumper.Active = true;
        _guardStickiness.StartWalkingTowards(Target);
    }

    private void Chase(Vector3 target)
    {
        if (Vector3.Distance(Transform.position, target) > 10)
        {
            _enemyJumper.JumpToPosition(target);
        }
    }

    private void StartReturning()
    {
        Attacking = false;
        _enemyJumper.Active = false;
        _guardStickiness.StartWalkingTowards(_initPos);
    }

    private void Return()
    {
        if (Vector2.Distance(Transform.position, _initPos) < 5)
        {
            _guardStickiness.StopWalkingTowards(false);
            SetState(StateType.Guard, null);
        }
    }

    public void Hear(HearingArea hearingArea)
    {
        Target = hearingArea.SourcePoint;

        if (IsState(StateType.Guard))
        {
            SetNextState(StateType.Check);
            _wonderTime = 2f;
            SetState(StateType.Wonder);
        }
        else
        {
            SetState(StateType.Chase);
        }
    }
}
