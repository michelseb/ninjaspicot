using System.Collections;
using UnityEngine;

public class RobotBall : Character, IViewer, IActivable, IWakeable
{
    [SerializeField] protected float _aimSpeed;
    [SerializeField] protected float _searchSpeed;
    public IKillable Target { get; set; }
    public bool Loaded { get; protected set; }
    public bool Active { get; protected set; }

    public float Range => 50f;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected Aim _aim;
    public Aim AimField { get { if (Utils.IsNull(_aim)) _aim = GetComponentInChildren<Aim>(); return _aim; } }

    protected Vector3 _targetLocation;
    protected Coroutine _wait;

    protected override void Awake()
    {
        base.Awake();
        _aimSpeed = .5f;
    }

    protected override void Start()
    {
        base.Start();
        Loaded = true;
        Deactivate();
    }

    protected virtual void Update()
    {
        Renderer.color = Active ? ColorUtils.Red : ColorUtils.White;

        if (!Active)
            return;
    }

    public void Activate()
    {
        Active = true;
        AimField.Activate();
        _characterLight.Wake();
    }

    public void Deactivate()
    {
        Active = false;
        AimField.Deactivate();
        _characterLight.Sleep();
    }

    public void Sleep()
    {
        Deactivate();
    }

    public void Wake()
    {
        Activate();
    }

    public override void Die(Transform killer)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Dying()
    {
        throw new System.NotImplementedException();
    }

    public void StartAim(IKillable target)
    {
        throw new System.NotImplementedException();
    }

    public void Aim(IKillable target)
    {
        throw new System.NotImplementedException();
    }

    public void LookFor()
    {
        throw new System.NotImplementedException();
    }
}