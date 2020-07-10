using UnityEngine;
using UnityEngine.UI;

public class Cannon : MonoBehaviour, IRaycastable
{
    private TurretBase _turret;
    public TurretBase Turret { get { if (_turret == null) _turret = GetComponentInParent<TurretBase>() ?? GetComponentInChildren<TurretBase>(); return _turret; } }
    public int Id => Turret.Id;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.color = Turret.Active ? ColorUtils.Red : ColorUtils.White;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Turret.Active)
            return;

        var hero = collision.collider.GetComponent<Hero>() ?? collision.collider.GetComponentInParent<Hero>();
        if (hero != null)
        {
            hero.Die(transform);
        }
    }
}
