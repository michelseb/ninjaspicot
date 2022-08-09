using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Components.Skills
{
    public interface ISkill : IDynamic, IComponent
    {
        /// <summary>
        /// Skill owner
        /// </summary>
        public ISkilled Owner { get; } 

        /// <summary>
        /// Skill active state
        /// </summary>
        public bool Active { get; }

        /// <summary>
        /// Set active state
        /// </summary>
        /// <param name="active"></param>
        void SetActive(bool active);

        /// <summary>
        /// Set skill owner
        /// </summary>
        /// <param name="owner"></param>
        void SetOwner(ISkilled owner);
    }
}