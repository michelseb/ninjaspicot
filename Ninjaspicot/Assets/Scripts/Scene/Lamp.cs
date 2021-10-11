using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour, ISceneryWakeable
{
    public Animator Animator { get; private set; }

    protected Light2D _light;
    public Light2D Light { get { if (Utils.IsNull(_light)) _light = gameObject.GetComponent<Light2D>(); return _light; } }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public bool StayOn { get; set; }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    public virtual void Wake()
    {
        Animator.SetTrigger("TurnOn");
    }

    public virtual void Sleep()
    {
        if (!StayOn)
        {
            Animator.SetTrigger("TurnOff");
        }
    }

    public void SetColor(Color color)
    {
        Light.color = color;
    }
}
