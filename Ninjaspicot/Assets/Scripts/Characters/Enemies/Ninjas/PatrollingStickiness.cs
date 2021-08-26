public class PatrollingStickiness : Stickiness
{
    public override bool Attach(Obstacle obstacle)
    {
        if (!base.Attach(obstacle))
            return false;

        StartWalking();

        return true;
    }
}
