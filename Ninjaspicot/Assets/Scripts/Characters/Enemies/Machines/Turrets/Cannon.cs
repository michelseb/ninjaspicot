using UnityEngine;
using UnityEngine.UI;

public class Cannon : TurretWall
{
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

        if (!collision.collider.CompareTag("hero"))
            return;

        Hero.Instance.Die();
    }
}
