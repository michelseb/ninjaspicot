using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;
using ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Components;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Decisions
{
    [CreateAssetMenu(fileName = "ViewDecision", menuName = "Zeplink/AI/Decisions/View decision")]
    public class ViewDecision : Decision
    {
        public override bool Decide(SmartEntity entity)
        {
            var visionComponent = entity.GetComponent<Aim>();

            return false;//visionComponent.TargetInView;
        }
    }
}