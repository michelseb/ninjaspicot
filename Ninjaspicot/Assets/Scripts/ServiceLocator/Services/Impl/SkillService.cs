using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Skills;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class SkillService : ComponentService, ISkillService
    {
        public T EquipSkill<T>(Transform instance) where T : SkillBase, new()
        {
            if (!instance.TryGetComponent(out ISkilled skilled))
                return default;

            var result = Equip<T>(instance);
            result.SetOwner(skilled);

            return result;
        }
    }
}
