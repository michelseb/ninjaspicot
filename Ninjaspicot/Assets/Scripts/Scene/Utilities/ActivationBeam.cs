using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public enum AccessGrant
{
    None = 0,
    Yes = 1,
    No = 2
}

public class ActivationBeam : MonoBehaviour, IWakeable
{
    [SerializeField] protected GameObject _activableObject;
    public bool Colliding { get; set; }

    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected IActivable _activable;
    protected Image _renderer;
    protected Lamp _light;
    protected AccessGrant _accessGrant;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _audioManager = AudioManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _light = GetComponentInChildren<Lamp>();
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
        UpdateState(GetAccessGrant(collision));
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        Colliding = false;
    }

    protected virtual void UpdateState(AccessGrant accessGrant)
    {
        if (_activable == null)
            return;

        _accessGrant = accessGrant;

        SetActiveColor(accessGrant);

        if (accessGrant == AccessGrant.None)
            return;

        Colliding = true;

        if (accessGrant == AccessGrant.Yes)
        {
            _activable.Activate();
        }
        else
        {
            _activable.Deactivate();
        }
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

    public void Sleep()
    {
        _light.enabled = false;
    }

    public void Wake()
    {
        _light.enabled = true;
    }

    protected AccessGrant GetAccessGrant(Collider2D collider)
    {
        if (Colliding)
            return _accessGrant;

        if (collider.CompareTag("hero"))
            return AccessGrant.No;

        if (collider.CompareTag("Enemy"))
            return AccessGrant.Yes;

        return AccessGrant.None;
    }
}
