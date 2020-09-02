using UnityEngine;

public class Aim : FieldOfView
{
    protected TurretBase _turret;
    public TurretBase Turret { get { if (_turret == null) _turret = GetComponentInParent<TurretBase>() ?? GetComponentInChildren<TurretBase>(); return _turret; } }

    public string CurrentTarget { get; set; }
    public bool TargetInRange { get; internal set; }
    public bool TargetInView { get; internal set; }

    protected virtual void Update()
    {
        Active = Turret.Active;

        var dist = Vector3.Distance(Hero.Instance.transform.position, _transform.position - _offset);
        TargetInRange = dist < _size;
        if (!TargetInRange && TargetInView)
        {
            TargetInView = false;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
        if (!Turret.Active || !collider.CompareTag("hero"))
            return;

        if (!string.IsNullOrEmpty(CurrentTarget) && collider.CompareTag(CurrentTarget))
        {
            var target = collider.GetComponent<IKillable>();
            if (target == null)
                return;

            Turret.Target = target;
            TargetInView = true;

            if (TargetAimedAt(target, Turret.Id))
            {
                Turret.StartAim(target);
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("hero") || Turret.Target == null)
            return;

        if (TargetAimedAt(Turret.Target, Turret.Id))
        {
            Turret.StartAim(Turret.Target);
        }
        TargetInView = true;
    }

    protected override void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.CompareTag("hero"))
            return;

        base.OnTriggerExit2D(collider);
        TargetInView = false;
    }

    public bool TargetCentered(Transform origin, string targetTag, int ignoreId = 0)
    {
        if (!TargetInView)
            return false;

        var collider = Utils.RayCast(origin.position, origin.up, ignore: ignoreId, ignoreType: typeof(TurretWall)).collider;

        if (collider == null)
            return false;

        return collider.CompareTag(targetTag);
    }

    public bool TargetAimedAt(IKillable target, int ignoreId = 0)
    {
        if (target == null || !TargetInView)
            return false;

        var hit = Utils.LineCast(_transform.position, target.Transform.position, ignoreId, false, target.Transform.tag, typeof(TurretWall));

        return hit && hit.transform.CompareTag(target.Transform.tag);
    }

    public bool TargetVisible(IKillable target, int ignoreId = 0)
    {
        if (target == null || !TargetInRange)
            return false;

        var hit = Utils.LineCast(_transform.position, target.Transform.position, ignoreId, false, target.Transform.tag, typeof(TurretWall));

        return hit && hit.transform.CompareTag(target.Transform.tag);
    }
}
