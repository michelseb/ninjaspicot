using System.Collections;
using UnityEngine;

public abstract class Enemy : Character, IWakeable, IFocusable, IResettable
{
    [SerializeField] protected ReactionType _reactionType;
    [SerializeField] protected Collider2D _castArea;
    protected Reaction _reaction;
    public bool Attacking { get; protected set; }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public bool Sleeping { get; set; }

    protected Vector3 _initPosition;
    protected Quaternion _initRotation;

    protected override void Start()
    {
        base.Start();
        SetReaction(_reactionType);
        _initPosition = Transform.position;
        _initRotation = Transform.rotation;
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


    //public virtual void Deactivate()
    //{
    //    gameObject.SetActive(false);
    //}

    //public virtual void Activate()
    //{
    //    gameObject.SetActive(true);
    //}

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

        Collider.enabled = false;
    }

    public virtual void Wake()
    {
        // This game is not called the walking dead!
        if (Dead)
            return;

        if (Renderer != null)
        {
            Renderer.enabled = true;
        }
        else if (Image != null)
        {
            Image.enabled = true;
        }
    }

    public void SetReaction(ReactionType reactionType)
    {
        if (_reaction != null && _reactionType == reactionType)
            return;

        string reaction = string.Empty;
        _reactionType = reactionType;
        switch (reactionType)
        {
            case ReactionType.Sleep:
                reaction = "Zzz";
                break;
            case ReactionType.Wonder:
                reaction = "??";
                break;
            case ReactionType.Find:
                reaction = "!!";
                break;
            case ReactionType.Patrol:
                reaction = "🐱‍";
                break;
        }

        if (!Utils.IsNull(_reaction))
        {
            _reaction.Deactivate();
        }
        _reaction = _poolManager.GetPoolable<Reaction>(transform.position, Quaternion.identity, 1f/Transform.lossyScale.magnitude, parent: Transform, defaultParent: false);
        _reaction.SetReaction(reaction);
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1)
    {
        if (!Utils.IsNull(_reaction))
        {
            _reaction.Transform.parent = null;
            _reaction.Deactivate();
            _reaction = null;
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

    public abstract void DoReset();
}
