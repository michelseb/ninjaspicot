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

        Renderer.color = Attacking ? ColorUtils.Red : ColorUtils.White;
    }

    public void Pool(Vector3 position, Quaternion rotation)
    {
        gameObject.SetActive(true);
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
        while (Renderer.color.a > 0)
        {
            Renderer.color = new Color(Renderer.color.r, Renderer.color.g, Renderer.color.b, Renderer.color.a - Time.deltaTime);
            yield return null;
        }
        Deactivate();
        yield return base.Dying();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
