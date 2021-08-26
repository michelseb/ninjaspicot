using System.Collections;
using UnityEngine;

public abstract class EnemyNinja : Enemy, INinja, IRaycastable, IPoolable
{
    public virtual PoolableType PoolableType => PoolableType.EnemyNinja;

    private EnemyJumper _jumper;
    public Jumper Jumper { get { if (Utils.IsNull(_jumper)) _jumper = GetComponent<EnemyJumper>() ?? GetComponentInChildren<EnemyJumper>(); return _jumper; } }

    private Stickiness _stickiness;
    public Stickiness Stickiness { get { if (Utils.IsNull(_stickiness)) _stickiness = GetComponent<Stickiness>() ?? GetComponentInChildren<Stickiness>(); return _stickiness; } }

    protected AudioClip _slash;
    private Canvas _canvas;

    protected override void Start()
    {
        base.Start();

        _slash = _audioManager.FindByName("Slash");
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = _cameraBehaviour.MainCamera;
    }

    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        Transform.position = position;
        Transform.rotation = rotation;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (Dead || Hero.Instance.Dead)
            return;

        if (collision.CompareTag("hero"))
        {
            var hero = collision.GetComponent<Hero>();
            if (Utils.IsNull(hero))
                return;

            if (!Attacking)
            {
                Die();
            }
            else
            {
                _audioSource.PlayOneShot(_slash, .3f);
                hero.Die(Transform);
            }
        }
    }

    public override void Die(Transform killer = null)
    {
        if (Dead)
            return;

        Dead = true;
        Stickiness.Detach();
        Jumper?.CancelJump();
        SetAllBehavioursActivation(false, false);
        StartCoroutine(Dying());
    }

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
        yield return StartCoroutine(base.Dying());
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
