using UnityEngine;

public class Aim : FieldOfView
{
    public bool TargetInRange { get; internal set; }
    public bool TargetInView { get; internal set; }

    protected virtual void Update()
    {
        if (!Active)
            return;

        var dist = Vector3.Distance(Hero.Instance.transform.position, _transform.position);
        TargetInRange = dist < _size;
        if (!TargetInRange && TargetInView)
        {
            TargetInView = false;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        if (!collider.CompareTag("hero"))
            return;

        Viewer.See(Hero.Instance.Transform);
    }

    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.CompareTag("hero"))
            return;

        Viewer.See(Hero.Instance.Transform);
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

        var collider = Utils.RayCast(origin.position, origin.up, ignore: ignoreId).collider;

        if (collider == null)
            return false;

        return collider.CompareTag(targetTag);
    }

    public bool TargetAimedAt(IKillable target, int ignoreId = 0)
    {
        if (target == null || !TargetInView)
            return false;

        var hit = Utils.LineCast(_transform.position, target.Transform.position, new int[] { ignoreId }, false, target.Transform.tag);

        return hit && hit.transform.CompareTag(target.Transform.tag);
    }

    public bool TargetVisible(IKillable target, int ignoreId = 0)
    {
        if (target == null || !TargetInRange)
            return false;

        var hit = Utils.LineCast(_transform.position, target.Transform.position, new int[] { ignoreId }, false, target.Transform.tag);

        return hit && hit.transform.CompareTag(target.Transform.tag);
    }
}
