using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour, IActivable
{
    protected Light2D _light;

    protected virtual void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    public virtual void Activate()
    {
        _light.enabled = true;
    }

    public virtual void Deactivate()
    {
        _light.enabled = false;
    }
}
