using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Actions
{
    public abstract class StateAction : ScriptableObject
    {
        public abstract void Execute(SmartEntity entity);
    }
}