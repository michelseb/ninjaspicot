using UnityEngine;

public abstract class HearingPerimeter : MonoBehaviour
{
    public virtual float Size => _listener?.Range ?? 1f;

    protected Transform _transform;
    protected IListener _listener;

    protected virtual void Awake()
    {
        _transform = transform;
        _listener = GetComponent<IListener>() ?? GetComponentInChildren<IListener>() ?? GetComponentInParent<IListener>();
    }

    protected virtual void Start()
    {
        _transform.localScale = Vector3.one * Size;
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("Sound"))
            return;

        _listener.Hear(collider.transform.position);
    }
}
