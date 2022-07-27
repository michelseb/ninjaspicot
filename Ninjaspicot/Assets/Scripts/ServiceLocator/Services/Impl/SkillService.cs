using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class SkillService : InstantiatorService<ISkill>, ISkillService
    {
        protected override string ModelPath => "";

        public T EquipSkill<T>(Transform instance) where T : class, ISkill
        {
            return Equip(typeof(T), instance) as T;
        }
    }
}
