using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GrapplingGun : Dynamic
{
    [SerializeField] private float _cordSpeed;
    [SerializeField] private float _bodyAccelerationSpeed;
    [SerializeField] private float _bodyMaxSpeed;
    [SerializeField] private Transform _shootOrigin;
    public bool Active { get; set; }
    public Trajectory Trajectory { get; protected set; }
    public Vector3 ShootOrigin => _shootOrigin.position;

    private Image _renderer;
    public Image Renderer { get { if (Utils.IsNull(_renderer)) _renderer = GetComponentInChildren<Image>(); return _renderer; } }

    public IFocusable Focusable => Trajectory?.Target;

    private LineRenderer _rope;
    private Hero _hero;
    private Coroutine _throwing;
    private Coroutine _pulling;
    private Rigidbody2D _rigidbody;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private Audio _throwSound;
    private Audio _hitSound;
    private TimeManager _timeManager;
    private CameraBehaviour _cameraBehaviour;
    private PoolManager _poolManager;
    private TouchManager _touchManager;
    private float _initGravity;

    public const float CHARGE_LENGTH = 50f;
    public const float MIN_DIST_TO_ACTIVATE = 10f;

    private void Awake()
    {
        _rope = GetComponentInChildren<LineRenderer>();
        _hero = Hero.Instance;
        _rigidbody = _hero.Rigidbody;
        _rope.positionCount = 2;
        _audioManager = AudioManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _timeManager = TimeManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _poolManager = PoolManager.Instance;
        _touchManager = TouchManager.Instance;
        _throwSound = _audioManager.FindAudioByName("GrapplingThrow");
        _hitSound = _audioManager.FindAudioByName("GrapplingHit");
    }

    private void Start()
    {
        Active = true;
        _initGravity = _rigidbody.gravityScale;
        Hide();
    }

    private void Update()
    {
        if (_throwing == null && _pulling == null && _touchManager.JumpDirection.HasValue)
        {
            Transform.rotation = Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, -_touchManager.JumpDirection.Value);
        }
    }

    public void StartThrow(IFocusable target)
    {
        if (_throwing != null)
            return;

        Trajectory.Disable();
        _audioManager.PlaySound(_audioSource, _throwSound);
        _throwing = StartCoroutine(DoThrow(target));
        Transform.rotation = Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, target.Transform.position - Transform.position);
    }

    private IEnumerator DoThrow(IFocusable target)
    {
        _rope.SetPosition(0, _shootOrigin.position);
        _rope.SetPosition(1, _shootOrigin.position);

        var cordCurrentPosition = _rope.GetPosition(1);
        while (Vector3.Distance(cordCurrentPosition, target.Transform.position) > 1f)
        {
            var nextPos = Vector3.MoveTowards(cordCurrentPosition, target.Transform.position, Time.deltaTime * _cordSpeed);
            _rope.SetPosition(0, _shootOrigin.position);
            _rope.SetPosition(1, nextPos);
            cordCurrentPosition = nextPos;

            yield return null;
        }

        _rope.SetPosition(1, target.Transform.position);
        StartPull(target);

        _throwing = null;
    }

    public void StartPull(IFocusable target)
    {
        if (_pulling != null)
            return;

        _audioManager.PlaySound(_audioSource, _hitSound);
        _pulling = StartCoroutine(DoPull(target));
    }

    private IEnumerator DoPull(IFocusable target)
    {
        var stickiness = _hero.Stickiness;
        float currentSpeed = 0f;

        if (target.Charge)
        {
            currentSpeed = _bodyMaxSpeed;
            _hero.StartDisplayGhosts(true, .01f);
        }

        var direction = (target.Transform.position - Transform.position).normalized;
        stickiness.CurrentAttachment?.LaunchQuickDeactivate();
        stickiness.Detach();

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.gravityScale = 0;

        while (Vector3.Dot(direction, target.Transform.position - 3 * direction - Transform.position) > 0.5f && !_hero.Stickiness.Attached)
        {
            if (currentSpeed < _bodyMaxSpeed)
            {
                currentSpeed += _bodyAccelerationSpeed * Time.deltaTime;
            }
            else
            {
                currentSpeed = _bodyMaxSpeed;
            }

            _rigidbody.velocity = direction * currentSpeed;

            _rope.SetPosition(0, _shootOrigin.position);
            Transform.rotation = Quaternion.Euler(0, 0, 90f) * Quaternion.LookRotation(Vector3.forward, target.Transform.position - Transform.position);

            yield return null;
        }

        if (target.Charge)
        {
            _cameraBehaviour.DoShake(.3f, .5f);
            _hero.StopDisplayGhosts();
            Bounce(target.Transform.position);
            _timeManager.SlowDown();
            _timeManager.StartTimeRestore();
        }

        target.GoTo();

        ReinitGravity();
        Hide();
    }

    public virtual void Cancel()
    {
        if (Trajectory == null || !Trajectory.Active)
            return;

        Trajectory.Disable();
        _timeManager.SetNormalTime();
        Hide();
    }

    public void ReinitGravity()
    {
        _rigidbody.gravityScale = _initGravity;
    }

    protected Trajectory GetTrajectory()
    {
        if (TrajectoryInUse())
        {
            Trajectory.Disable();
        }

        if (Trajectory == null || !Trajectory.Active)
            return _poolManager.GetPoolable<Trajectory>(_shootOrigin.position, Quaternion.identity);

        Trajectory.ReUse(Transform.position);
        return Trajectory;
    }

    public void Bounce(Vector3 targetPosition)
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(((Transform.position - targetPosition).normalized + Vector3.up * 2) * 30, ForceMode2D.Impulse);
    }

    public virtual bool CanInit()
    {
        //if (CompareTag("Dynamic"))
        //    return Hero.Instance.Jumper.CanInitJump();

        if (!Active)
            return false;

        return true;
    }

    public virtual bool IsReady()
    {
        //if (CompareTag("Dynamic"))
        //    return Hero.Instance..ReadyToJump();

        return CanInit() && TrajectoryInUse() && Trajectory.Aiming;
    }

    public Trajectory SetTrajectory()
    {
        if (TrajectoryInUse())
            return Trajectory;

        Trajectory = GetTrajectory();
        return Trajectory;
    }

    public bool TrajectoryInUse()
    {
        return Trajectory != null && Trajectory.Used;
    }

    public void Show()
    {
        Renderer.enabled = true;
        _rope.enabled = true;
    }

    public void Hide()
    {
        Renderer.enabled = false;
        _rope.enabled = false;
        _pulling = null;
        _throwing = null;
        ReinitGravity();
        StopAllCoroutines();
    }
}
