using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Bonuses
{
    public class ExtraJump : Bonus, IResettable
    {
        public override void DoReset()
        {
            if (_temporaryDeactivate != null)
            {
                StopCoroutine(_temporaryDeactivate);
            }

            Activate();
        }

        public override void TakeBy(IPicker picker)
        {
            if (picker is not ISkilled skilled)
                return;

            skilled.GetSkill<HopSkill>()?.GainJumps(1);
            skilled.GetSkill<ChargeSkill>()?.GainJumps(1);
            _audioService.PlaySound(this, "ExtraJump", .5f);

            base.TakeBy(picker);
        }
    }
}