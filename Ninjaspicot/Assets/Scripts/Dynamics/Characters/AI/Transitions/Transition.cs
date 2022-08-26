using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Decisions;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;
using ZepLink.RiceNinja.Dynamics.Characters.AI.States;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Transitions
{
    [CreateAssetMenu(fileName = "Transition", menuName = "Zeplink/AI/Transition")]
    public sealed class Transition : ScriptableObject
    {
        public Decision Decision;
        public BaseState YesState;
        public BaseState NoState;

        public void Execute(SmartEntity entity)
        {
            if (Decision.Decide(entity) && YesState is not RemainInState)
            {
                entity.CurrentState = YesState;
            }
            else if (!Decision.Decide(entity) && NoState is not RemainInState)
            {
                entity.CurrentState = NoState;
            }
        }
    }
}