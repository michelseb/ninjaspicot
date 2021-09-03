using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : Enemy, IActivable, IRaycastable, IViewer, IListener
{
    public enum Mode { Scan, LookFor, Aim, Wonder };

    [SerializeField] protected float _viewAngle;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected bool _clockWise;
    [SerializeField] protected float _initRotation;
    [SerializeField] protected float _aimSpeed;
    [SerializeField] protected float _searchSpeed;
    [SerializeField] protected float _wonderTime;

    public IKillable TargetEntity { get; set; }
    public bool Loaded { get; protected set; }
    public Mode TurretMode { get; protected set; }
    public bool Active { get; protected set; }
    public Aim AimField => _aim;

    public float Range => 50f;

    protected Aim _aim;
    protected Image _image;
    protected Vector3 _targetLocation;
    protected Coroutine _wait;

    protected override void Awake()
    {
        base.Awake();
        _aim = GetComponentInChildren<Aim>();
        _aim.CurrentTarget = "hero";
        _image = GetComponent<Image>();
        _aimSpeed = .5f;
    }

    protected override void Start()
    {
        base.Start();
        TurretMode = Mode.Scan;
        Loaded = true;
        Activate();
        Transform.Rotate(0, 0, _initRotation);

    }

    protected virtual void Update()
    {
        _image.color = Active ? ColorUtils.Red : ColorUtils.White;

        if (!Active)
            return;

        switch (TurretMode)
        {
            case Mode.Aim:
                Aim(TargetEntity);
                break;

            case Mode.Scan:
                Scan();
                break;

            case Mode.LookFor:
                LookFor();
                break;


            case Mode.Wonder:
                Wonder();
                break;
        }

    }

    public virtual void Aim(IKillable target)
    {
        var speed = _aim.TargetInView ? _aimSpeed : _searchSpeed;
        Transform.rotation = Quaternion.RotateTowards(Transform.rotation, Quaternion.LookRotation(Vector3.forward, target.Transform.position - Transform.position), speed);
    }

    public virtual void LookFor()
    {
        Transform.rotation = Quaternion.RotateTowards(Transform.rotation, Quaternion.LookRotation(Vector3.forward, _targetLocation - Transform.position), _searchSpeed);
    }

    protected virtual void Scan()
    {
        var dir = _clockWise ? 1 : -1;
        Transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime * dir);

        if (dir * (Transform.rotation.eulerAngles.z - _initRotation) > _viewAngle)
        {
            _clockWise = !_clockWise;
        }
    }

    protected virtual void Wonder()
    {
        if (TargetEntity != null && _aim.TargetVisible(TargetEntity, Id))
        {
            StartAim(TargetEntity);
        }
    }

    public void StartAim(IKillable target)
    {
        TargetEntity = target;
        TurretMode = Mode.Aim;

        if (_wait != null)
        {
            StopCoroutine(_wait);
            _wait = null;
        }
    }

    public void StartWait()
    {
        TurretMode = Mode.Wonder;
        _wait = StartCoroutine(Wait());
    }

    protected virtual IEnumerator Wait()
    {
        yield return new WaitForSeconds(_wonderTime);

        if (TurretMode != Mode.Aim)
        {
            TurretMode = Mode.Scan;
        }
    }

    public override void Activate()
    {
        Active = true;
        _aim.Activate();
    }

    public override void Deactivate()
    {
        Active = false;
        _aim.Deactivate();
    }

    public void Hear(HearingArea hearingArea)
    {
        if (TurretMode == Mode.Aim || !Active)
            return;

        StopAllCoroutines();
        _targetLocation = hearingArea.SourcePoint;
        TurretMode = Mode.LookFor;
    }

    public override void Wake()
    {
        Activate();
    }

    public override void Sleep()
    {
        Deactivate();
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