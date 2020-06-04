using UnityEngine;

public class Cannon : MonoBehaviour, IRaycastable
{
    private Turret _turret;
    public Turret Turret { get { if (_turret == null) _turret = transform.parent.GetComponent<Turret>(); return _turret; } }
    public int Id => Turret.Id;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var hero = collision.collider.GetComponent<Hero>() ?? collision.collider.GetComponentInParent<Hero>();
        if (hero != null)
        {
            hero.Die(transform);
        }
    }
}
