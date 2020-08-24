using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, IRaycastable, IActivable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    protected Collider2D _collider;
    public Transform Transform { get; private set; }

    protected virtual void Awake()
    {
        Transform = transform;
        _collider = GetComponent<Collider2D>() ?? GetComponentInChildren<Collider2D>() ?? GetComponentInParent<Collider2D>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        var ninja = collision.gameObject.GetComponent<Ninja>();
        
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
            Physics2D.IgnoreCollision(_collider, heroCollider, false);
        }
    }

    public virtual void Deactivate()
    {
        var heroCollider = Hero.Instance?.Stickiness?.Collider;
        if (heroCollider != null)
        {
            Physics2D.IgnoreCollision(_collider, heroCollider);
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
