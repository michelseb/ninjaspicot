public class PatrollingEnemy : EnemyNinja
{
    protected override void Start()
    {
        base.Start();
        Attacking = true;
        Stickiness.StartWalking();
    }

    public override bool NeedsToWalk()
    {
        return true;
    }
}
