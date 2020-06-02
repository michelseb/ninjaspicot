using UnityEngine;

public class Aim : MonoBehaviour
{
    private Turret _turret;

    private void Start()
    {
        _turret = transform.parent.GetComponent<Turret>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            var hit = Utils.LineCast(_turret.gameObject.transform.position, collision.gameObject.transform.position, gameObject.GetInstanceID(), false, true, true);

            if (hit.transform.CompareTag("hero"))
            {
                if (!_turret.AutoShoot)
                {
                    _turret.StartAim(hit.transform);
                }
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            if (!_turret.AutoShoot)
            {
                _turret.StartSearch();
            }
        }
    }
}
