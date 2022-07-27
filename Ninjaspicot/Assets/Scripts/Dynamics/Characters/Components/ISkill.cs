using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Components
{
    public interface ISkill : IDynamic
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
    }
}