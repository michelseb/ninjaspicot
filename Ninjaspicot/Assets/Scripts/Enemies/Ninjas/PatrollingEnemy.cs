public class PatrollingEnemy : EnemyNinja
{
    protected override void Start()
    {
        base.Start();
        SetAttacking(true);
        Stickiness.StartWalking();
    }

    public override bool NeedsToWalk()
    {
        return true;
    }
}
