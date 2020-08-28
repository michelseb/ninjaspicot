using UnityEngine;

public class HeroJumper : Jumper
{
    protected DynamicInteraction _dynamicInteraction;
    protected TimeManager _timeManager;
    protected CameraBehaviour _cameraBehaviour;

    protected override void Awake()
    {
        base.Awake();
        _dynamicEntity = GetComponent<IDynamic>();
        _dynamicInteraction = GetComponent<DynamicInteraction>();
        _cameraBehaviour = CameraBehaviour.Instance;
        _timeManager = TimeManager.Instance;
    }

    public override void NormalJump(Vector2 direction)
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
        base.NormalJump(direction);
        Trajectory = null;
    }

    public override bool CanJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.Jumper.CanJump();

        if (!Active || GetJumps() <= 0)
            return false;

        return !Utils.BoxCast(transform.position, Vector2.one, 0f, TrajectoryOrigin - TrajectoryDestination, 5f, Hero.Instance.Id,
        layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));
    }

    public override bool ReadyToJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.Jumper.ReadyToJump();

        return base.ReadyToJump();
    }

    public override void CancelJump()
    {
        base.CancelJump();
        _timeManager.SetNormalTime();
    }

    public void SetJumpPositions(Vector3 origin, Vector3 destination)
    {
        TrajectoryOrigin = origin;
        TrajectoryDestination = destination;
    }
}
