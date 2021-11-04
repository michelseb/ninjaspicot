using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : Dynamic, ISceneryWakeable, IFocusable, IActivable, IResettable
{
    public Animator Animator { get; private set; }

    protected Light2D _light;
    public Light2D Light { get { if (Utils.IsNull(_light)) _light = gameObject.GetComponent<Light2D>(); return _light; } }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public bool StayOn { get; set; }
    public bool IsSilent => false;
    public bool Taken => false;
    public bool FocusedByNormalJump => false;

    protected bool _broken;


    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
    }

    public virtual void Wake()
    {
        if (_broken)
            return;

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

    public void Activate()
    {
        _broken = true;
        Hero.Instance.Jumper.Bounce(Transform.position);
        Sleep();
    }

    public void Deactivate() { }

    public void DoReset()
    {
        _broken = false;
        Wake();
    }
}