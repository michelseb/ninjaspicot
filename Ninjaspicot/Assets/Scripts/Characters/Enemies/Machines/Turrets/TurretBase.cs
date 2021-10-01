using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : Enemy, IActivable, IRaycastable, IViewer
{
    public enum Mode { Scan, LookFor, Aim, Wonder };

    [SerializeField] protected float _viewAngle;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected bool _clockWise;
    [SerializeField] protected float _initRotation;
    [SerializeField] protected float _aimSpeed;
    [SerializeField] protected float _searchSpeed;
    [SerializeField] protected float _wonderTime;
    [SerializeField] protected Transform _shootingPosition;
    [SerializeField] protected Transform _turretHead;

    public IKillable TargetEntity { get; set; }
    public bool Loaded { get; protected set; }
    public Mode TurretMode { get; protected set; }
    public bool Active { get; protected set; }
    public Aim AimField => _aim;

    public float Range => 50f;
    public bool Seeing { get; set; }

    protected Aim _aim;
    protected Image _image;
    protected Vector3 _targetLocation;
    protected Coroutine _wait;

    protected override void Awake()
    {
        base.Awake();
        _aim = GetComponentInChildren<Aim>();
        _aim.CurrentTarget = "hero";
        _image = _turretHead.GetComponent<Image>();
        _aimSpeed = .5f;
    }

    protected override void Start()
    {
        base.Start();
        TurretMode = Mode.Scan;
        Loaded = true;
        Activate();
        _turretHead.Rotate(0, 0, _initRotation);

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
        _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, Quaternion.LookRotation(Vector3.forward, target.Transform.position - _turretHead.position), speed);
    }

    public virtual void LookFor()
    {
        _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, Quaternion.LookRotation(Vector3.forward, _targetLocation - _turretHead.position), _searchSpeed);
    }

    protected virtual void Scan()
    {
        var dir = _clockWise ? 1 : -1;
        _turretHead.Rotate(0, 0, _rotationSpeed * Time.deltaTime * dir);

        if (dir * (_turretHead.rotation.eulerAngles.z - _initRotation) > _viewAngle)
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

    public void See(Transform target)
    {
        if (target.TryGetComponent(out IKillable killable))
        {
            StartAim(killable);
        }
    }

    public virtual void Activate()
    {
        Active = true;
        _aim.Activate();
    }

    public virtual void Deactivate()
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

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1f)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator Dying()
    {
        throw new System.NotImplementedException();
    }
}