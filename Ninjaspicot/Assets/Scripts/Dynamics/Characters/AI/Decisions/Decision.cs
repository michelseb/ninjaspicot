using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.AI.Entities;

namespace ZepLink.RiceNinja.Dynamics.Characters.AI.Decisions
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(SmartEntity entity);
    }
}