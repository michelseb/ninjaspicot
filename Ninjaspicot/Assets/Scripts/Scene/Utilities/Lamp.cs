using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : Dynamic, ISceneryWakeable, IResettable
{
    private Animator _animator;
    public Animator Animator { get { if (Utils.IsNull(_animator)) _animator = gameObject.GetComponent<Animator>(); return _animator; } }

    protected Light2D _light;
    public Light2D Light { get { if (Utils.IsNull(_light)) _light = gameObject.GetComponent<Light2D>(); return _light; } }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public bool StayOn { get; set; }
    public bool IsSilent => false;
    public bool Taken => Broken;

    public bool Broken { get; set; }

    public bool Charge => true;

    public virtual void Wake()
    {
        if (Broken)
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

    public void DoReset()
    {
        Animator.enabled = true;
        Light.enabled = true;
        Broken = false;
        Wake();
    }

    public bool Break(Transform killer = null, Audio sound = null, float volume = 1)
    {
        if (Broken)
            return false;

        Animator.enabled = false;
        Light.enabled = false;
        Hero.Instance.GrapplingGun.Bounce(Transform.position);
        Broken = true;

        return true;
    }

    public void GoTo()
    {
        Break();
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (Broken || !collision.CompareTag("hero"))
    //        return;

    //    VisibilityManager.AddRevelator();
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (Broken || !collision.CompareTag("hero"))
    //        return;

    //    VisibilityManager.RemoveRevelator();
    //}
}