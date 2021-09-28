using UnityEngine;

public abstract class HearingPerimeter : MonoBehaviour, IActivable
{
    public virtual float Size => _listener?.Range ?? 1f;
    public SoundMark SoundMark { get; private set; }

    protected Transform _transform;
    protected IListener _listener;
    protected PoolManager _poolManager;
    protected bool _isActive;

    protected virtual void Awake()
    {
        _poolManager = PoolManager.Instance;
        _transform = transform;
        _listener = GetComponent<IListener>() ?? GetComponentInChildren<IListener>() ?? GetComponentInParent<IListener>();
    }

    protected virtual void Start()
    {
        _transform.localScale = Vector3.one * Size;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider) // Before => Stay2D
    {
        if (!collider.CompareTag("Sound") || !_isActive)
            return;

        _listener.Hear(new HearingArea
        {
            SourcePoint = collider.transform.position
        });

        if (!Utils.IsNull(SoundMark))
        {
            SoundMark.Deactivate();
        }

        SoundMark = _poolManager.GetPoolable<SoundMark>(Hero.Instance.Transform.position, Quaternion.identity);
    }

    public void Activate()
    {
        _isActive = true;
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        _isActive = false;
        gameObject.SetActive(false);
    }
}
