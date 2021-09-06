using UnityEngine;

public class RobotBall : Enemy, IActivable
{
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected Transform _sprite;

    public bool Active { get; protected set; }
    protected Rigidbody2D _rigidbody;

    protected FieldOfView _fieldOfView;
    public FieldOfView FieldOfView { get { if (Utils.IsNull(_fieldOfView)) _fieldOfView = GetComponentInChildren<FieldOfView>(); return _fieldOfView; } }
    protected EnemyLaser _laser;
    public EnemyLaser Laser { get { if (Utils.IsNull(_laser)) _laser = GetComponentInChildren<EnemyLaser>(); return _laser; } }

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();
        Activate();
    }

    protected virtual void Update()
    {
        if (!Active)
            return;

        var rot = _sprite.rotation.eulerAngles.z;
        Renderer.flipY = rot > 90 && rot < 270;
    }

    public override void Activate()
    {
        Active = true;
        Laser?.Activate();
        FieldOfView.Activate();
        _reaction?.Activate();
        _characterLight.Wake();
    }

    public override void Deactivate()
    {
        Active = false;
        Renderer.color = ColorUtils.White;
        FieldOfView.Deactivate();
        Laser?.Deactivate();
        _reaction?.Deactivate();
        _characterLight.Sleep();
    }

    public override void Sleep()
    {
        Deactivate();
    }

    public override void Wake()
    {
        Activate();
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1f)
    {
        Deactivate();
    }

}