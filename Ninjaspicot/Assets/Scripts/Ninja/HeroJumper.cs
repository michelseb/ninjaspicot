using UnityEngine;

public class HeroJumper : Jumper
{
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

    private void Update()
    {
        if (!Active)
            return;

        if (_touchManager.Touching)
        {
            if (CanJump())
            {
                _trajectory = GetTrajectory();
                _trajectory.DrawTrajectory(transform.position, _touchManager.TouchDrag, _touchManager.RawTouchOrigin, _strength);
            }
            else if (_trajectory != null)
            {
                ReinitJump();
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            if (ReadyToJump())
            {
                if (_trajectory != null && !_trajectory.IsClear(transform.position, 2))//Add ninja to new layer
                {
                    _trajectory.ReinitTrajectory();
                    _timeManager.SetNormalTime();
                }
                else
                {
                    _stickiness.StopWalking(false);
                    Jump(_touchManager.RawTouchOrigin, _touchManager.TouchDrag, _strength);
                }
                _touchManager.ReinitDrag();
            }
        }
    }

    public override void Jump(Vector2 origin, Vector2 drag, float strength)
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
        base.Jump(origin, drag, strength);
    }

    public override bool CanJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.CanJump();

        var boxCast = Utils.BoxCast(transform.position, Vector2.one, 0f, _touchManager.RawTouchOrigin - _touchManager.TouchDrag, 5f, Hero.Instance.Id/*, display: true*/,
             layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));

        return GetJumps() > 0 && _touchManager.Dragging && !boxCast;
    }

    public override bool ReadyToJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.ReadyToJump();

        return CanJump() && _trajectory != null;
    }
}
