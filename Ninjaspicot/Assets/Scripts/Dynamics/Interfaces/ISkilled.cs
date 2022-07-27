using System.Collections.Generic;
using System.Linq;
using ZepLink.RiceNinja.Dynamics.Characters.Components;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface ISkilled : IDynamic
    {
        /// <summary>
        /// List of owned skills
        /// </summary>
        IList<ISkill> Skills { get; }

        /// <summary>
        /// Get first skill of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetSkill<T>() where T : class, ISkill
        {
            return Skills?.FirstOrDefault(s => s is T) as T;
        }

        /// <summary>
        /// Get all skills of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetSkills<T>() where T : class, ISkill
        {
            return Skills.Where(s => s is T).Cast<T>().ToArray();
        }
    }
}