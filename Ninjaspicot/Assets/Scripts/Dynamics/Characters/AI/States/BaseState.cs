using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.States
{
    public class BaseState : ScriptableObject
    {
        public virtual void Execute(SmartEntity entity) { }
    }
}