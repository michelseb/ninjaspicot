using UnityEngine;

public class Aim : FieldOfView
{
    protected TurretBase _turret;
    public TurretBase Turret { get { if (_turret == null) _turret = GetComponentInParent<TurretBase>() ?? GetComponentInChildren<TurretBase>(); return _turret; } }

    public string CurrentTarget { get; set; }
    public bool TargetInRange { get; internal set; }

    protected virtual void Update()
    {
        Active = Turret.Active;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (!Turret.Active)
            return;

        if (!string.IsNullOrEmpty(CurrentTarget) && collision.CompareTag(CurrentTarget))
        {
            TargetInRange = true;
            if (TargetAimedAt(collision.transform, Turret.Id))
            {
                Turret.StartAim(collision.transform);
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
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

        var hit = Utils.LineCast(_transform.position, target.position, ignoreId, false, target.tag);

        return hit && hit.transform.CompareTag(target.tag);
    }
}
