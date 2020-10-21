using UnityEngine;

public class TurretAim : Aim
{
    protected TurretBase _turret;

    protected override void Awake()
    {
        base.Awake();
        _turret = Viewer as TurretBase;
    }

    protected override void Update()
    {
        Active = _turret.Active;

        if (Active)
        {
            base.Update();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);
        if (!_turret.Active || !collider.CompareTag("hero"))
            return;

        if (!string.IsNullOrEmpty(CurrentTarget) && collider.CompareTag(CurrentTarget))
        {
            var target = collider.GetComponent<IKillable>();
            if (target == null)
                return;

            _turret.Target = target;
            TargetInView = true;

            if (TargetAimedAt(target, Viewer.Id))
            {
                _turret.StartAim(target);
            }
        }
    }

    protected override void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("hero") || _turret.Target == null)
            return;

        if (TargetAimedAt(_turret.Target, Viewer.Id))
        {
            _turret.StartAim(_turret.Target);
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
}
