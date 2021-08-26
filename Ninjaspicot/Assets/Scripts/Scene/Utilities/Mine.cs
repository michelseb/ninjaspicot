using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class Mine : MonoBehaviour, IActivable, IWakeable
{
    protected Image _renderer;
    protected Color _initialColor;
    protected PoolManager _poolManager;
    protected AudioSource _audioSource;
    protected AudioManager _audioManager;
    protected Light2D _light;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _poolManager = PoolManager.Instance;
        _renderer = GetComponent<Image>();
        _initialColor = _renderer.color;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _light = GetComponent<Light2D>();
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("hero") || Hero.Instance.Dead)
            return;
        _poolManager.GetPoolable<Explosion>(transform.position, transform.rotation);
        _audioManager.PlaySound(_audioSource, "Explode");
        Hero.Instance.Die(transform);
    }

    public virtual void Activate()
    {
        _renderer.color = ColorUtils.Blue;
    }

    public virtual void Deactivate()
    {
        _renderer.color = _initialColor;
    }

    public void Sleep()
    {
        _light.enabled = false;
    }

    public void Wake()
    {
        _light.enabled = true;
    }
}
