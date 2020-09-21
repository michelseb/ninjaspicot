using System.Collections;
using UnityEngine;

public abstract class EnemyNinja : Ninja, IRaycastable, IPoolable, IWakeable
{
    [SerializeField] protected Collider2D _bottom;
    public bool Attacking { get; protected set; }
    public virtual PoolableType PoolableType => PoolableType.EnemyNinja;

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
                Die(hero.transform);
            }
            else
            {
                _audioSource.PlayOneShot(_slash, .3f);
                hero.Die(Transform);
            }
        }
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
        Deactivate();
        yield return StartCoroutine(base.Dying());
    }

    protected void SetAttacking(bool attacking)
    {
        Attacking = attacking;

        if (Renderer != null)
        {
            Renderer.color = Attacking ? ColorUtils.Red : ColorUtils.White;
        }
        if (Image != null)
        {
            Image.color = Attacking ? ColorUtils.Red : ColorUtils.White;
        }
    }


    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Sleep()
    {
        throw new System.NotImplementedException();
    }

    public void Wake()
    {
        throw new System.NotImplementedException();
    }
}
