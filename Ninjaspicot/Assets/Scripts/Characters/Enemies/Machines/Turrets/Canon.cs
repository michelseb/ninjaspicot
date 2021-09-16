using UnityEngine;

public class Canon : MonoBehaviour
{
    private TurretBase _turret;

    private void Awake()
    {
        _turret = GetComponentInParent<TurretBase>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_turret.Active)
            return;

        if (!collision.collider.CompareTag("hero"))
            return;

        Hero.Instance.Die();
    }
}
