using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class TurretBase : MonoBehaviour, IActivable, IRaycastable, IListener, IWakeable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public enum Mode { Scan, LookFor, Aim, Wonder };

    [SerializeField] protected float _viewAngle;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected bool _clockWise;
    [SerializeField] protected float _initRotation;
    [SerializeField] protected float _aimSpeed;
    [SerializeField] protected float _searchSpeed;
    [SerializeField] protected float _wonderTime;

    public IKillable Target { get; set; }
    public bool Loaded { get; protected set; }
    public Mode TurretMode { get; protected set; }
    public bool Active { get; protected set; }
    public Aim AimField => _aim;

    public float Range => 50f;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected Aim _aim;
    protected Image _image;
    protected Vector3 _targetLocation;
    protected Coroutine _wait;
    protected Transform _transform;

    protected virtual void Awake()
    {
        _aim = GetComponentInChildren<Aim>();
        _aim.CurrentTarget = "hero";
        _image = GetComponent<Image>();
        _transform = transform; //Caching transform for enhanced performance
        _aimSpeed = .5f;
    }

    protected virtual void Start()
    {
        TurretMode = Mode.Scan;
        Loaded = true;
        Activate();
        _transform.Rotate(0, 0, _initRotation);

    }

    protected virtual void Update()
    {
        _image.color = Active ? ColorUtils.Red : ColorUtils.White;

        if (!Active)
            return;

        switch (TurretMode)
        {
            case Mode.Aim:
                Aim();
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

    protected virtual void Aim()
    {
        var speed = _aim.TargetInView ? _aimSpeed : _searchSpeed;
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(Vector3.forward, Target.Transform.position - _transform.position), speed);
    }

    protected virtual void LookFor()
    {
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Quaternion.LookRotation(Vector3.forward, _targetLocation - _transform.position), _searchSpeed);
    }

    protected virtual void Scan()
    {
        var dir = _clockWise ? 1 : -1;
        _transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime * dir);

        if (dir * (_transform.rotation.eulerAngles.z - _initRotation) > _viewAngle)
        {
            _clockWise = !_clockWise;
        }
    }

    protected virtual void Wonder()
    {
        if (Target != null && _aim.TargetVisible(Target, Id))
        {
            StartAim(Target);
        }
    }

    public void StartAim(IKillable target)
    {
        Target = target;
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

    public void Activate()
    {
        Active = true;
        _aim.Activate();
    }

    public void Deactivate()
    {
        Active = false;
        _aim.Deactivate();
    }

    public void Hear(Vector3 source)
    {
        if (TurretMode == Mode.Aim || !Active)
            return;

        StopAllCoroutines();
        _targetLocation = source;
        TurretMode = Mode.LookFor;
    }

    public void Sleep()
    {
        Deactivate();
    }

    public void Wake()
    {
        Activate();
    }
}