using System.Collections;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Obstacle : MonoBehaviour, IRaycastable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    protected Collider2D _collider;

    public virtual void Awake()
    {
        _collider = GetComponents<Collider2D>().FirstOrDefault(c => !c.isTrigger);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        var ninja = collision.gameObject.GetComponent<Ninja>();

        if (ninja == null)
            return;

        ninja.Stickiness.ReactToObstacle(this);
        ninja.Stickiness.CurrentAttachment = this;

        ninja.Stickiness.SetContactPoint(collision.contacts[collision.contacts.Length - 1]);
    }

    public void LaunchQuickDeactivate()
    {
        StartCoroutine(QuickDeactivate());
    }

    private IEnumerator QuickDeactivate()
    {
        _collider.enabled = false;
        yield return new WaitForSecondsRealtime(.1f);
        _collider.enabled = true;
    }
}
