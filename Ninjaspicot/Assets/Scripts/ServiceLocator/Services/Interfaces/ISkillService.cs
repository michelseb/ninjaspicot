using System;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ISkillService : IInstantiatorService<ISkill>
    {
        /// <summary>
        /// Adds a skill to an instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        T EquipSkill<T>(Transform instance) where T : class, ISkill;
    }
}