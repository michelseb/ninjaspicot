using System.Collections;
using UnityEngine;

public abstract class Enemy : Character, IWakeable
{
    public bool Attacking { get; protected set; }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public override IEnumerator Dying()
    {
        if (Renderer != null)
        {
            while (Renderer.color.a > 0)
            {
                Renderer.color = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, Renderer.color.a - Time.deltaTime);
                yield return null;
            }
        }
        else if (Image != null)
        {
            while (Image.color.a > 0)
            {
                Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, Image.color.a - Time.deltaTime);
                yield return null;
            }
        }
        Deactivate();
    }

    protected void SetAttacking(bool attacking)
    {
        Attacking = attacking;

        if (Renderer != null)
        {
            Renderer.color = Attacking ? ColorUtils.Red : ColorUtils.White;
        }
        else if (Image != null)
        {
            Image.color = Attacking ? ColorUtils.Red : ColorUtils.White;
        }
    }


    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Sleep()
    {
        if (Renderer != null)
        {
            Renderer.enabled = false;
        }
        else if (Image != null)
        {
            Image.enabled = false;
        }
    }

    public virtual void Wake()
    {
        if (Renderer != null)
        {
            Renderer.enabled = true;
        }
        else if (Image != null)
        {
            Image.enabled = true;
        }
    }
}
