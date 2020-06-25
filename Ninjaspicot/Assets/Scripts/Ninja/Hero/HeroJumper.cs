﻿using UnityEngine;

public class HeroJumper : Jumper
{
    public bool NeedsJump => NeedsJump1 || NeedsJump2;
    public bool NeedsJump1 { get; set; }
    public bool NeedsJump2 { get; set; }
    public Vector3 TrajectoryOrigin { get; set; }
    public Vector3 TrajectoryDestination { get; set; }
    private DynamicInteraction _dynamicInteraction;
    private TimeManager _timeManager;
    private CameraBehaviour _cameraBehaviour;
    private TouchManager _touchManager;

    protected override void Awake()
    {
        base.Awake();
        _dynamicEntity = GetComponent<IDynamic>();
        _dynamicInteraction = GetComponent<DynamicInteraction>();
        _cameraBehaviour = CameraBehaviour.Instance;
        _timeManager = TimeManager.Instance;
        _touchManager = TouchManager.Instance;
    }

    public override void Jump(Vector2 origin, Vector2 drag)
    {
        if (_dynamicInteraction.Interacting)
        {
            _dynamicInteraction.StopInteraction(true);
        }

        if (GetJumps() <= 0)
        {
            _timeManager.SetNormalTime();
        }

        _cameraBehaviour.DoShake(.3f, .1f);
        base.Jump(origin, drag);
        Trajectory = null;
        NeedsJump1 = false;
        NeedsJump2 = false;
    }

    public override bool CanJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.CanJump();

        if (!Active || GetJumps() <= 0 || !NeedsJump)
            return false;

        return !Utils.BoxCast(transform.position, Vector2.one, 0f, TrajectoryOrigin - TrajectoryDestination, 5f, Hero.Instance.Id,
        layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));
    }

    public override bool ReadyToJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.ReadyToJump();

        return CanJump() && Trajectory != null;
    }

    public void SetJumpPositions(Vector3 origin, Vector3 destination)
    {
        TrajectoryOrigin = origin;
        TrajectoryDestination = destination;
    }
}
