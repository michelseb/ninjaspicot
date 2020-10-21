public class PatrollingStickiness : Stickiness
{
    public override void Attach(Obstacle obstacle)
    {
        base.Attach(obstacle);
        StartWalking();
    }
}
