public class ExtraJump : Bonus, IResettable
{
    public virtual void DoReset()
    {
        if (_temporaryDeactivate != null)
        {
            StopCoroutine(_temporaryDeactivate);
        }

        Activate();
        //_animator.Rebind();
        //_animator.Update(0);
        //_animator.Play(_animator.GetCurrentAnimatorStateInfo(0).fullPathHash);
    }

    public override void Take()
    {
        _hero.Jumper.GainJumps(1);
        _audioManager.PlaySound(_audioSource, "ExtraJump", .5f);

        base.Take();
    }
}
