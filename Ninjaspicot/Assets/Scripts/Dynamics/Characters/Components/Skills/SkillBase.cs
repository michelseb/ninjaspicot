using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Characters.Components.Skills
{
    public abstract class SkillBase : Physic, ISkill
    {
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