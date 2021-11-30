using UnityEngine;

public class AimableLocation : Dynamic, IFocusable, ISceneryWakeable, IActivable
{
    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    public bool IsSilent => true;

    public bool Active { get; set; }
    private bool _taken;
    public virtual bool Taken => !IsAvailable();

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public virtual bool Charge => false;

    protected SpriteRenderer _renderer;
    protected Collider2D _collider;
    protected Lamp _lamp;

    protected virtual void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _lamp = GetComponentInChildren<Lamp>();
        Deactivate();
    }

    protected virtual void Update()
    {
        if (_taken != Taken)
        {
            if (Taken)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }

            _taken = Taken;
        }
    }

    public void Activate()
    {
        _renderer.color = ColorUtils.Yellow;
        _lamp.Wake();
    }

    public void Deactivate()
    {
        _renderer.color = ColorUtils.Black;
        _lamp.Sleep();
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

        var circleRay = Utils.CircleCast(heroPos + direction * GrapplingGun.MIN_DIST_TO_ACTIVATE, 2, direction, dist - 2 * GrapplingGun.MIN_DIST_TO_ACTIVATE, Hero.Id, false);
        if (circleRay)
            return false;

        return (Utils.LineCast(Transform.position, heroPos).transform.GetInstanceID() == _hero.Id);
    }

    public virtual void GoTo()
    {
    }
}
