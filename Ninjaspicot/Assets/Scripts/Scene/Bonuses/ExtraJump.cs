public class ExtraJump : Bonus
{
    public override void Take()
    {
        _hero.Jumper.GainJumps(1);
        _audioManager.PlaySound(_audioSource, "ExtraJump", .5f);

        base.Take();
    }
}
