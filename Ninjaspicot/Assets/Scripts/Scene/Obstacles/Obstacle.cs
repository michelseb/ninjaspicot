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

        ninja.Stickiness.ReactToObstacle(this, collision.contacts[collision.contacts.Length - 1].point);
    }
}
