using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components;

namespace ZepLink.RiceNinja.Dynamics.Characters.Ninjas.Components
{
    public class HopSkill : JumpSkill<HopTrajectory>
    {
        protected override string _soundName => "Jump";

        protected override float _soundIntensity => .3f;

        public override void Jump(Vector2 direction)
        {
            if (!Ready)
                return;
        }
    }
}