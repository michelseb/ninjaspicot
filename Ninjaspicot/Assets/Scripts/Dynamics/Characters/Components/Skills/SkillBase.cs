using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Components.Skills
{
    public abstract class SkillBase : Physic, ISkill
    {
        public override int Id => Owner.Id;
        public override Transform Parent => Owner.Transform;
        public ISkilled Owner { get; protected set; }
        public bool Active { get; protected set; }

        public virtual void SetActive(bool active)
        {
            Active = active;
        }

        public virtual void SetOwner(ISkilled owner)
        {
            Owner = owner;
        }
    }
}