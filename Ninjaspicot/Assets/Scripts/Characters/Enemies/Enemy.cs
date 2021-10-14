using System.Collections;
using UnityEngine;

public abstract class Enemy : Character, ISceneryWakeable, IFocusable, IResettable
{
    [SerializeField] protected ReactionType _initReactionType;
    [SerializeField] protected Collider2D _castArea;
    protected Reaction _reaction;
    public bool Attacking { get; protected set; }
    public bool Active { get; protected set; }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    #region IFocusable
    public bool IsSilent => false;
    public bool Taken => true;
    #endregion

    protected Vector3 _resetPosition;
    protected Quaternion _resetRotation;

    protected override void Start()
    {
        base.Start();
        SetReaction(_initReactionType);
        _resetPosition = Transform.position;
        _resetRotation = Renderer?.transform.rotation ?? Image.transform.rotation;
    }

    public override IEnumerator Dying()
    {
        var col = Renderer?.color ?? Image.color;
        var alpha = col.a;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime;
            if (Renderer != null)
            {
                Renderer.color = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, Renderer.color.a - Time.deltaTime);
            }
            else if (Image != null)
            {
                Image.color = new Color(Image.color.r, Image.color.g, Image.color.b, Image.color.a - Time.deltaTime);
            }

            yield return null;
        }

        Die();
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

    public virtual void Sleep()
    {
        Active = false;

        if (Renderer != null)
        {
            Renderer.enabled = false;
        }
        else if (Image != null)
        {
            Image.enabled = false;
        }

        Collider.enabled = false;
        _characterLight.Sleep();
        if (!Utils.IsNull(_reaction)) _reaction?.Sleep();
    }

    public virtual void Wake()
    {
        // This game is not called the walking dead!
        if (Dead)
            return;

        Active = true;

        if (Renderer != null)
        {
            Renderer.enabled = true;
        }
        else if (Image != null)
        {
            Image.enabled = true;
        }

        _characterLight.Wake();
        if (!Utils.IsNull(_reaction)) _reaction.Wake();
    }

    public void SetReaction(ReactionType reactionType)
    {
        if (Utils.IsNull(_reaction))
        {
            _reaction = _poolManager.GetPoolable<Reaction>(Transform.position, Quaternion.identity, 1f / Transform.lossyScale.magnitude, parent: Transform, defaultParent: false);
        }
        else if (_reaction.ReactionType == reactionType)
        {
            return;
        }

        _reaction.SetReaction(reactionType);
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1)
    {
        if (!Utils.IsNull(_reaction))
        {
            _reaction.Transform.parent = null;
            _reaction.Sleep();
        }

        if (!Utils.IsNull(_castArea))
        {
            _castArea.enabled = false;
        }

        if (!Utils.IsNull(Collider))
        {
            Collider.enabled = false;
        }

        Dead = true;
        Sleep();
    }

    public virtual void DoReset()
    {
        Attacking = false;
        SetReaction(_initReactionType);
        Transform.position = _resetPosition;

        if (Renderer != null)
        {
            Renderer.transform.rotation = _resetRotation;
        }
        else
        {
            Image.transform.rotation = _resetRotation;
        }

        Dead = false;
        Wake();
    }
}
