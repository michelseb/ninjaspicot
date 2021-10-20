using UnityEngine;

public abstract class RobotBall : Enemy
{
    [SerializeField] protected float _rotateSpeed;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected Transform _sprite;
    [SerializeField] protected ParticleSystem _electricity;

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

    protected virtual void Update()
    {
        if (!Active)
            return;

        var rot = _sprite.rotation.eulerAngles.z;
        Renderer.flipY = rot > 90 && rot < 270;
    }

    public override void Sleep()
    {
        base.Sleep();

        FieldOfView.Deactivate();
        Laser?.Deactivate();
        _castArea.enabled = false;
        _electricity.Stop();
    }

    public override void Wake()
    {
        base.Wake();

        _castArea.enabled = true;
        _electricity.Play();

        if (_initState != StateType.Sleep)
        {
            FieldOfView.Activate();
        }
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1f)
    {
        if (Dead)
            return;

        if (killer != null)
        {
            _rigidbody.AddForce((Transform.position - killer.position).normalized * 50, ForceMode2D.Impulse);
        }

        base.Die(killer, sound, volume);
    }

    public override void DoReset()
    {
        StopAllCoroutines();
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0;
        Laser.SetActive(false);
        base.DoReset();
    }
}