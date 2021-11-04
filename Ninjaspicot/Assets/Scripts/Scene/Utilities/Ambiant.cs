using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Ambiant : MonoBehaviour, ISceneryWakeable, IResettable
{
    protected Animator _animator;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    private Light2D _light;
    private Color _initColor;
    private float _initIntensity;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _light = GetComponent<Light2D>();
        _initColor = _light.color;
        _initIntensity = _light.intensity;
    }

    public void Wake()
    {
        _animator.SetTrigger("Wake");
    }

    public void Sleep()
    {
        _animator.SetTrigger("Sleep");
    }

    public void SetColor(CustomColor color, float intensity = 1)
    {
        _light.color = ColorUtils.GetColor(color);
        _light.intensity = intensity;
    }

    public void DoReset()
    {
        _light.color = _initColor;
        _light.intensity = _initIntensity;
    }
}
