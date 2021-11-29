using UnityEngine;

public class AimableLocation : Dynamic, IFocusable, ISceneryWakeable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    public bool IsSilent => true;

    public bool Active { get; set; }
    public bool Taken => !IsAvailable();

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public virtual bool Charge => false;

    protected Animator _animator;
    protected SpriteRenderer _renderer;
    protected Collider2D _collider;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    protected virtual void Update()
    {
        _renderer.color = Taken ? ColorUtils.Black : ColorUtils.Yellow;
        _collider.enabled = !Taken;
    }


    public void Sleep()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
        Active = false;
    }

    public void Wake()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        Active = true;
    }


    private bool IsAvailable()
    {
        if (!Active)
            return false;

        var heroPos = Hero.Rigidbody.position;
        var direction = (Utils.ToVector2(Transform.position) - heroPos).normalized;
        var dist = Vector3.Distance(heroPos, Transform.position);
        if (dist < GrapplingGun.MIN_DIST_TO_ACTIVATE || dist > GrapplingGun.CHARGE_LENGTH)
            return false;

        var boxRay = Utils.BoxCast(heroPos + direction * GrapplingGun.MIN_DIST_TO_ACTIVATE, Vector2.one * 5, 0, direction, dist - 2 * GrapplingGun.MIN_DIST_TO_ACTIVATE, Hero.Id, true, false);
        if (boxRay)
            return false;

        var lineRay = Utils.LineCast(heroPos, Transform.position, new int[] { _hero.Id });

        if (lineRay)
            return false;

        return true;
    }

    public virtual void GoTo()
    {
    }
}
