using UnityEngine;

public abstract class EnemyNinja : Enemy, INinja, IRaycastable
{
    private EnemyJumper _jumper;
    public Jumper Jumper { get { if (Utils.IsNull(_jumper)) _jumper = GetComponent<EnemyJumper>() ?? GetComponentInChildren<EnemyJumper>(); return _jumper; } }

    private Stickiness _stickiness;
    public Stickiness Stickiness { get { if (Utils.IsNull(_stickiness)) _stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>(); return _stickiness; } }

    protected Audio _slash;
    private Canvas _canvas;

    protected override void Start()
    {
        base.Start();

        _slash = _audioManager.FindAudioByName("Slash");
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = _cameraBehaviour.MainCamera;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Dead || Hero.Instance.Dead)
            return;

        if (collision.CompareTag("hero") && collision.TryGetComponent(out Hero hero))
        {
            if (!Attacking)
            {
                Die();
            }
            else
            {
                _audioManager.PlaySound(_audioSource, _slash, .3f);
                hero.Die(Transform);
            }
        }
    }

    public override void Die(Transform killer = null, Audio sound = null, float volume = 1f)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        Jumper?.CancelJump();
        SetAllBehavioursActivation(false, false);
        StartCoroutine(Dying());
    }

    public void SetJumpingActivation(bool active)
    {
        if (!Utils.IsNull(Jumper))
        {
            Jumper.Active = active;
        }
    }

    public void SetStickinessActivation(bool active)
    {
        if (!Utils.IsNull(Stickiness))
        {
            Stickiness.Active = active;
        }
    }

    public void SetWalkingActivation(bool active, bool grounded)
    {
        if (Utils.IsNull(Stickiness))
            return;

        if (!active)
        {
            Stickiness.StopWalking(grounded);
        }

        Stickiness.CanWalk = active;
    }

    public virtual void SetAllBehavioursActivation(bool active, bool grounded)
    {
        SetJumpingActivation(active);
        SetStickinessActivation(active);
        SetWalkingActivation(active, grounded);
    }

    public virtual bool NeedsToWalk()
    {
        return false;
    }
}
