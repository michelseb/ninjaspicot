using System.Collections;
using UnityEngine;

public abstract class HearingPerimeter : MonoBehaviour
{
    public virtual float Size => _listener?.Range ?? 1f;
    public SoundMark SoundMark { get; private set; }

    protected Transform _transform;
    protected IListener _listener;
    protected PoolManager _poolManager;

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
        if (!collider.CompareTag("Sound"))
            return;

        var soundEffect = collider.GetComponent<SoundEffect>();

        if (Utils.IsNull(soundEffect))
            return;

        var pos = collider.transform.position;

        StartCoroutine(SetHearingArea(soundEffect, pos));
    }

    private IEnumerator SetHearingArea(SoundEffect soundEffect, Vector3 position)
    {
        float timer = 0f;

        while (soundEffect.CompositeContainer == null)
        {
            timer += Time.deltaTime;

            if (timer > .5f)
                yield break;

            yield return null;
        }

        _listener.Hear(new HearingArea
        {
            SourcePoint = position,
            ClosestLocation = soundEffect.CompositeContainer.GetClosestLocationPoint(position)
        });

        if (!Utils.IsNull(SoundMark))
        {
            SoundMark.Deactivate();
        }

        SoundMark = _poolManager.GetPoolable<SoundMark>(Hero.Instance.Transform.position, Hero.Instance.Transform.rotation);
    }
}
