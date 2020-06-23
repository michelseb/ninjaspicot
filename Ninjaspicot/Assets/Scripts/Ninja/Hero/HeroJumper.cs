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

                if (_touchManager.Dragging1)
                {
                    _trajectory.DrawTrajectory(transform.position, _touchManager.Touch1Drag, _touchManager.RawTouch1Origin, _strength);
                }
                else if (_touchManager.Dragging2)
                {
                    _trajectory.DrawTrajectory(transform.position, _touchManager.Touch2Drag, _touchManager.RawTouch2Origin, _strength);
                }
            }
            else if (_trajectory != null && _trajectory.Used)
            {
                ReinitJump();
            }
        }

        if (Input.GetButtonUp("Fire1") || _touchManager.TouchLifted)
        {
            if (ReadyToJump())
            {
                if (_trajectory.Used && !_trajectory.IsClear(transform.position, 2))//Add ninja to new layer
                {
                    _trajectory.StartFading();
                    _timeManager.SetNormalTime();
                }
                else
                {
                    _stickiness.StopWalking(false);

                    if (_touchManager.Dragging1)
                    {
                        Jump(_touchManager.RawTouch1Origin, _touchManager.Touch1Drag, _strength);
                        _touchManager.ReinitDrag1();
                    }
                    else if (_touchManager.Dragging2)
                    {
                        Jump(_touchManager.RawTouch2Origin, _touchManager.Touch2Drag, _strength);
                        _touchManager.ReinitDrag2();
                    }
                }
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
        _trajectory = null;
    }

    public override bool CanJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.CanJump();

        if (GetJumps() <= 0 || !_touchManager.Dragging)
            return false;

        if (_touchManager.Dragging1)
        {
            return !Utils.BoxCast(transform.position, Vector2.one, 0f, _touchManager.RawTouch1Origin - _touchManager.Touch1Drag, 5f, Hero.Instance.Id,
            layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));
        }
        else if (_touchManager.Dragging2)
        {
            return !Utils.BoxCast(transform.position, Vector2.one, 0f, _touchManager.RawTouch2Origin - _touchManager.Touch2Drag, 5f, Hero.Instance.Id,
            layer: (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("DynamicObstacle")) | (1 << LayerMask.NameToLayer("PoppingObstacle")));
        }

        return false;
    }

    public override bool ReadyToJump()
    {
        if (CompareTag("Dynamic"))
            return Hero.Instance.JumpManager.ReadyToJump();

        return CanJump() && _trajectory != null;
    }
}
