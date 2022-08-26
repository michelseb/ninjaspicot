using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Actions;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Transitions;
using ZepLink.RiceNinja.Dynamics.Effects;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.States
{
    [CreateAssetMenu(fileName = "State", menuName = "Zeplink/AI/States/New state")]
    public sealed class NewState : BaseState
    {
        public List<StateAction> Actions = new List<StateAction>();
        public List<Transition> Transitions = new List<Transition>();
        public StateEffect StateVisual;

        public override void Execute(SmartEntity entity)
        {
            base.Execute(entity);

            foreach (var action in Actions)
            {
                action.Execute(entity);
            }

            foreach (var transition in Transitions)
            {
                transition.Execute(entity);
            }
        }
    }
}