using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, IRaycastable, IActivable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    public Collider2D Collider { get; private set; }
    public Transform Transform { get; private set; }

    public CompositeContainer Composite { get; private set; }

    protected virtual void Awake()
    {
        Transform = transform;
        Collider = GetComponent<Collider2D>() ?? GetComponentInChildren<Collider2D>() ?? GetComponentInParent<Collider2D>();

        if (!Utils.IsNull(Collider) && (Collider is PolygonCollider2D || Collider is BoxCollider2D))
        {
            Collider.usedByComposite = true;
        }

        Composite = Collider.composite?.GetComponent<CompositeContainer>() ?? Collider.GetComponent<CompositeContainer>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        var ninja = collision.gameObject.GetComponent<INinja>();
        
        if (ninja == null)
            return;

        var contact = collision.contacts[0];

        ninja.Stickiness.ReactToObstacle(this, contact.point);
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
        yield return new WaitForSeconds(.1f);
        Activate();
    }
}
