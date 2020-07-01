using System.Collections;
using UnityEngine;

public abstract class EnemyNinja : Ninja, IRaycastable, IPoolable
{
    [SerializeField] protected Dir _direction;
    [SerializeField] protected Collider2D _bottom;
    public bool Attacking { get; protected set; }
    public virtual PoolableType PoolableType => PoolableType.EnemyNinja;

    protected virtual void Start()
    {
        Stickiness.NinjaDir = _direction;
    }

    protected virtual void Update()
    {
        if (Dead)
            return;

        if (Renderer != null)
        {
            Renderer.color = Attacking ? ColorUtils.Red : ColorUtils.White;
        }
        if (Image != null)
        {
            Image.color = Attacking ? ColorUtils.Red : ColorUtils.White;
        }
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        Activate();
        transform.position = position;
        transform.rotation = rotation;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (Dead)
            return;

        if (collision.collider.CompareTag("hero"))
        {
            var hero = collision.collider.GetComponent<Hero>();
            if (hero == null)
                return;

            if (!Attacking)
            {
                Die(hero.transform);
            }
            else
            {
                hero.Die(transform);
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
        yield return base.Dying();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
