using UnityEngine;
using UnityEngine.UI;

public enum AccessGrant
{
    None = 0,
    Yes = 1,
    No = 2
}

public class ActivationBeam : MonoBehaviour, IActivable, IRaycastable
{
    [SerializeField] protected GameObject _activableObject;
    private int _collidingAmount;
    public bool Colliding => _collidingAmount > 0;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected IActivable _activable;
    protected Image _renderer;
    protected Lamp _light;
    protected AccessGrant? _accessGrant;
    private ZoneManager _zoneManager;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    protected virtual void Awake()
    {
        _zoneManager = ZoneManager.Instance;
        _audioManager = AudioManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _light = GetComponentInChildren<Lamp>();
        _light.StayOn = true;
        _renderer = GetComponent<Image>();
        if (!_renderer)
        {
            _renderer = GetComponentInChildren<Image>();
        }
    }

    protected virtual void Start()
    {
        _activable = _activableObject?.GetComponent<IActivable>() ?? _activableObject?.GetComponentInChildren<IActivable>() ?? _activableObject?.GetComponentInParent<IActivable>();

        SetActiveColor(AccessGrant.None);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero") && !collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("hero"))
        {
            _zoneManager.SetZone(Zone);
        }

        _collidingAmount++;
        UpdateState(GetAccessGrant(collision.tag));
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero") && !collision.CompareTag("Enemy"))
            return;

        _collidingAmount--;
        UpdateState(GetAccessGrant(collision.tag));
    }

    protected virtual void SetActiveColor(AccessGrant accessGrant)
    {
        Color color = ColorUtils.White;
        switch (accessGrant)
        {
            case AccessGrant.None:
                color = ColorUtils.Blue;
                break;
            case AccessGrant.No:
                color = ColorUtils.Red;
                break;
            case AccessGrant.Yes:
                color = ColorUtils.Green;
                break;
        }

        _renderer.color = color;
        _light.SetColor(color);
    }

    protected AccessGrant GetAccessGrant(string entityTag)
    {
        //if (Colliding)
        //    return _accessGrant.Value;
        if (!Colliding)
            return AccessGrant.None;

        if (entityTag == null || entityTag == "hero")
            return AccessGrant.No;

        if (entityTag == "Enemy")
            return AccessGrant.Yes;

        return AccessGrant.None;
    }

    protected void UpdateState(AccessGrant accessGrant)
    {
        if (_collidingAmount > 1)
            return;

        if (accessGrant == AccessGrant.None)
        {
            ResetState();
        }
        else if (accessGrant == AccessGrant.Yes)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    public void ResetState()
    {
        _accessGrant = AccessGrant.None;
        SetActiveColor(AccessGrant.None);
    }

    public void Activate()
    {
        // Hack => if activated by charge
        if (!Colliding)
        {
            Deactivate();
            _zoneManager.SetZone(Zone);

            return;
        }
        _accessGrant = AccessGrant.Yes;
        SetActiveColor(AccessGrant.Yes);
        _activable.Activate();
    }

    public void Deactivate()
    {
        _accessGrant = AccessGrant.No;
        SetActiveColor(AccessGrant.No);
        _activable.Deactivate();
    }
}
