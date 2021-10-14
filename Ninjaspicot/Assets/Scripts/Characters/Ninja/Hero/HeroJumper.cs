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

    protected override void Start()
    {
        base.Start();
        Active = true;
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

    public override void Charge(Vector2 direction)
    {
        base.Charge(direction);
        _cameraBehaviour.DoShake(.3f, .5f);
    }

    public override bool CanJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.Jumper.CanJump();

        if (!Active || GetJumps() <= 0)
            return false;

        return !Utils.BoxCast(transform.position, Vector2.one, 0f, TrajectoryOrigin - TrajectoryDestination, 15f, Hero.Instance.Id,
        layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")));
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

    public override void CommitJump()
    {
        if (Trajectory == null || !Trajectory.Active)
            return;

        base.CommitJump();

        if (Trajectory is ChargeTrajectory chargeTrajectory)
        {
            chargeTrajectory.Bonuses.ForEach(x => x.Take());
            chargeTrajectory.Interactives.ForEach(x => x.Activate());

            if (chargeTrajectory.Target == null)
                return;

            chargeTrajectory.Target.Die(_transform);
            chargeTrajectory.Target = null;
            GainJumps(1);
            _timeManager.SlowDown();
            _timeManager.StartTimeRestore();

            //Bounce
            _stickiness.Rigidbody.velocity = Vector2.zero;
            _stickiness.Rigidbody.AddForce(((_stickiness.Transform.position - ((MonoBehaviour)chargeTrajectory.Target).transform.position).normalized + Vector3.up * 2) * 15, ForceMode2D.Impulse);
        }

    }
}
