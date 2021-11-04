using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : Enemy, IActivable, IRaycastable, IViewer
{
    public enum Mode { Scan, LookFor, Aim, Wonder };

    [SerializeField] protected float _viewAngle;
    [SerializeField] protected bool _clockWise;
    [SerializeField] protected float _initRotation;
    [SerializeField] protected float _wonderTime;
    [SerializeField] protected Transform _shootingPosition;
    [SerializeField] protected Transform _turretHead;

    public IKillable TargetEntity { get; set; }

    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    public bool Loaded { get; protected set; }
    public Mode TurretMode { get; protected set; }
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
        _image = _turretHead.GetComponent<Image>();
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
        _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, Quaternion.LookRotation(Vector3.forward, target.Transform.position - _turretHead.position), GetRotateSpeed());
    }

    public virtual void LookFor()
    {
        _turretHead.rotation = Quaternion.RotateTowards(_turretHead.rotation, Quaternion.LookRotation(Vector3.forward, _targetLocation - _turretHead.position), GetRotateSpeed());
    }

    protected virtual void Scan()
    {
        var dir = _clockWise ? 1 : -1;
        _turretHead.Rotate(0, 0, _rotateSpeed * Time.deltaTime * dir);

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
        // Visible when walking in the dark ?
        if (!Hero.Visible /*&& !Hero.Stickiness.Walking*/)
            return;

        if (!Active || !target.CompareTag("hero"))
            return;

        if (!target.TryGetComponent(out IKillable killable))
            return;


        TargetEntity = killable;
        AimField.TargetInView = true;

        if (AimField.TargetAimedAt(killable, Id))
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
        throw new NotImplementedException();
    }

    public override IEnumerator Dying()
    {
        throw new NotImplementedException();
    }

    protected override Action GetActionFromState(StateType stateType, StateType? nextState = null)
    {
        return null;
    }
}