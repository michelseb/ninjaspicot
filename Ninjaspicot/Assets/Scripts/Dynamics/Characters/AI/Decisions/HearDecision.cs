using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Hearing;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Decisions
{
    [CreateAssetMenu(fileName = "HearDecision", menuName = "Zeplink/AI/Decisions/Hear decision")]
    public class HearDecision : Decision
    {
        public override bool Decide(SmartEntity entity)
        {
            var hearingComponent = entity.GetComponent<HearingPerimeter>();

            return hearingComponent.Hearing;
        }
    }
}