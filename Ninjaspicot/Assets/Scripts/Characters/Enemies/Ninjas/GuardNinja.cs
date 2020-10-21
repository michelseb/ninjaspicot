﻿using System.Collections;
using UnityEngine;

public enum GuardMode
{
    Guarding,
    Wondering,
    Checking,
    Chasing,
    Returning
}

public class GuardNinja : EnemyNinja, IListener
{
    //[SerializeField] private FollowingRobotBall _robot;

    public GuardMode GuardMode { get; private set; }
    public Vector3 Target { get; private set; }
    public LocationPoint TargetLocation { get; private set; }

    private float _wonderTime;
    private Vector3 _initPos;
    private LocationPoint _initLocation;
    private GuardMode _nextState;
    private GuardStickiness _guardStickiness;
    private EnemyJumper _enemyJumper;
    private Reaction _reaction;
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
        GuardMode = GuardMode.Guarding;
    }

    protected virtual void Update()
    {
        if (_initLocation == null)
        {
            _initLocation = _guardStickiness.LocationPoint;
            _initPos = Transform.position;
        }

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
                Chase(Target);
                break;

            case GuardMode.Returning:
                Return();
                break;

        }
    }

    private void StartWondering()
    {
        GuardMode = GuardMode.Wondering;
        _reaction = _poolManager.GetPoolable<Reaction>(transform.position + transform.up * 10, transform.rotation);

        var reaction = _nextState == GuardMode.Checking ? "!" : "?";
        _reaction.SetReaction(reaction);

        _wonderElapsedTime = 0;
        //_robot.Activate();
    }

    private void Wonder()
    {
        _wonderElapsedTime += Time.deltaTime;

        if (_wonderElapsedTime >= _wonderTime)
        {
            _reaction?.Deactivate();

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
        Attacking = true;
        GuardMode = GuardMode.Checking;
        _nextState = GuardMode.Returning;
        _guardStickiness.StartWalkingTowards(TargetLocation, Target);
        //StartCoroutine(Check());
    }

    private void Check()
    {
        if (Vector2.Distance(Transform.position, Target) < 5)
        {
            _hearingPerimeter.SoundMark?.Deactivate();
            _guardStickiness.StopWalkingTowards(false);
            _wonderTime = 5f;
            StartWondering();
        }
    }

    //private IEnumerator Check()
    //{
    //    while (_guardMode == GuardMode.Checking)
    //    {
    //        //_robot.AddFollowPosition(Transform.position);
    //        yield return new WaitForSeconds(.3f);
    //    }
    //}

    private void StartChasing()
    {
        Attacking = true;
        _enemyJumper.Active = true;
        GuardMode = GuardMode.Chasing;
        _guardStickiness.StartWalkingTowards(TargetLocation, Target);
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
        GuardMode = GuardMode.Returning;
        _guardStickiness.StartWalkingTowards(_initLocation, _initPos);
    }

    private void Return()
    {
        if (Vector2.Distance(Transform.position, _initPos) < 5)
        {
            _guardStickiness.StopWalkingTowards(false);
            StartGuarding();
        }
    }

    private void StartGuarding()
    {
        GuardMode = GuardMode.Guarding;
    }

    public void Guard()
    {

    }

    public void Hear(HearingArea hearingArea)
    {
        Target = hearingArea.GetSource();
        TargetLocation = hearingArea.ClosestLocation;

        if (GuardMode == GuardMode.Guarding)
        {
            _nextState = GuardMode.Checking;
            _wonderTime = 1f;
            StartWondering();
        }
        else
        {
            StartChasing();
        }
    }
}
