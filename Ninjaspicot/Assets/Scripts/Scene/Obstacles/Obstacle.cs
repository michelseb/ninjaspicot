using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, IRaycastable
{
    protected int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

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
        var layer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        yield return new WaitForSecondsRealtime(.1f);
        gameObject.layer = layer;
    }
}
