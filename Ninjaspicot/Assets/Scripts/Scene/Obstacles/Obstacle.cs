using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, IRaycastable, IActivable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private Collider2D _collider;
    public Collider2D Collider => _collider;
    public Transform Transform { get; private set; }

    protected virtual void Awake()
    {
        Transform = transform;

        if (!TryGetComponent(out _collider))
        {
            _collider = GetComponentInChildren<Collider2D>() ?? GetComponentInParent<Collider2D>();
        }
        if (!Utils.IsNull(Collider) && (Collider is PolygonCollider2D || Collider is BoxCollider2D))
        {
            Collider.usedByComposite = true;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out INinja ninja))
        {
            ninja.Stickiness.ReactToObstacle(this, collision.contacts[0].point);
        }
    }

    public virtual void Activate()
    {
        var heroCollider = Hero.Instance?.Stickiness?.Collider;
        if (heroCollider != null)
        {
            Physics2D.IgnoreCollision(Collider, heroCollider, false);
        }
    }

    public virtual void Deactivate()
    {
        var heroCollider = Hero.Instance?.Stickiness?.Collider;
        if (heroCollider != null)
        {
            Physics2D.IgnoreCollision(Collider, heroCollider);
        }
    }

    public void LaunchQuickDeactivate()
    {
        StartCoroutine(QuickDeactivate());
    }

    private IEnumerator QuickDeactivate()
    {
        Deactivate();
        yield return new WaitForSeconds(1f);
        Activate();
    }
}
