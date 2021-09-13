using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ActivationButton : MonoBehaviour, IWakeable
{
    [SerializeField] protected bool _active;
    [SerializeField] protected GameObject _activableObject;
    [SerializeField] protected bool _triggerOnStart;
    [SerializeField] protected bool _desactivationButton;
    [SerializeField] protected bool _activableByEnemy;
    public bool Pressing { get; set; }

    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected IActivable _activable;
    protected SpriteRenderer _renderer;
    protected Light2D _light;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }
    public bool Sleeping { get; set; }

    protected virtual void Awake()
    {
        _audioManager = AudioManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _light = GetComponent<Light2D>();
        _renderer = GetComponent<SpriteRenderer>();
        if (!_renderer) 
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    protected virtual void Start()
    {
        _activable = _activableObject?.GetComponent<IActivable>() ?? _activableObject?.GetComponentInChildren<IActivable>() ?? _activableObject?.GetComponentInParent<IActivable>();

        SetActiveColor(_active);

        if (_triggerOnStart)
        {
            SetActive(_active);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Pressed(collision))
        {
            ToggleActivation(_active);
            Pressing = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        Pressing = false;
    }

    protected virtual void ToggleActivation(bool active)
    {
        active = !active;
        SetActive(active);
    }

    protected virtual void SetActive(bool active)
    {
        if (_activable == null)
            return;

        _active = active;
        SetActiveColor(active);

        if (_desactivationButton)
        {
            active = !active;
        }

        if (active)
        {
            _activable.Activate();
        }
        else
        {
            _activable.Deactivate();
        }
    }

    protected virtual void SetActiveColor(bool active)
    {
        _renderer.color = active ? ColorUtils.Green : ColorUtils.Red;
        _light.color = active ? ColorUtils.Green : ColorUtils.Red;
    }

    public void Sleep()
    {
        _light.enabled = false;
    }

    public void Wake()
    {
        _light.enabled = true;
    }

    protected bool Pressed(Collider2D collider)
    {
        return !Pressing && (collider.CompareTag("hero") || (_activableByEnemy && collider.CompareTag("Enemy")));
    }
}
