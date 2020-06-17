using UnityEngine;

public class Aim : FieldOfView
{
    private Turret _turret;
    public Turret Turret { get { if (_turret == null) _turret = GetComponentInParent<Turret>() ?? GetComponentInChildren<Turret>(); return _turret; } }

    public string CurrentTarget { get; set; }
    public bool TargetInRange { get; internal set; }

    private void Update()
    {
        Active = Turret.Active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Turret.Active)
            return;

        if (!string.IsNullOrEmpty(CurrentTarget) && collision.CompareTag(CurrentTarget))
        {
            TargetInRange = true;
            if (TargetAimedAt(collision.transform, Turret.Id))
            {
                if (!Turret.AutoShoot)
                {
                    Turret.StartAim(collision.transform);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(CurrentTarget) && collision.CompareTag(CurrentTarget))
        {
            TargetInRange = false;
        }
    }

    public bool TargetCentered(Transform origin, string targetTag, int ignoreId = 0)
    {
        if (!TargetInRange)
            return false;

        var collider = Utils.RayCast(origin.position, origin.up, ignore: ignoreId).collider;

        if (collider == null)
            return false;

        return collider.CompareTag(targetTag);
    }

    public bool TargetAimedAt(Transform target, int ignoreId = 0)
    {
        if (target == null || !TargetInRange)
            return false;

        var hit = Utils.LineCast(transform.position, target.position, ignoreId, false, target.tag);

        return hit && hit.transform.CompareTag(target.tag);
    }
}
