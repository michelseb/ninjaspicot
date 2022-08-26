using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Actions
{
    [CreateAssetMenu(fileName = "PatrolAction", menuName = "Zeplink/AI/Actions/Patrol action")]
    public class PatrolAction : StateAction
    {
        public override void Execute(SmartEntity entity)
        {
            entity.Smart.Patrol();
            //var movement = entity.Smart.GetMovementType();

            //entity.Smart.LaunchMovement(movement);
        }
    }
}