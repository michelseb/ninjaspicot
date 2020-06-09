using UnityEngine;

public class Perimeter : MonoBehaviour, IRaycastable
{
    private Turret _turret;
    public Turret Turret { get { if (_turret == null) _turret = transform.parent.GetComponent<Turret>(); return _turret; } }
    public int Id => Turret.Id;
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    var hero = collision.GetComponent<Hero>() ?? collision.GetComponentInParent<Hero>();
    //    if (hero != null)
    //    {
    //        hero.Die(transform);
    //    }
    //}
}
