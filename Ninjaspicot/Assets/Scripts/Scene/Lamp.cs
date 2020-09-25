using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour, IWakeable
{
    protected Animator _animator;

    protected Light2D _light;
    public Light2D Light { get { if (Utils.IsNull(_light)) _light = gameObject.GetComponent<Light2D>(); return _light; } }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public virtual void Wake()
    {
        _animator.SetTrigger("TurnOn");
    }

    public virtual void Sleep()
    {
        _animator.SetTrigger("TurnOff");
    }
}
