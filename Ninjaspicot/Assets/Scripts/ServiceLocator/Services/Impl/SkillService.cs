using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Characters.Components.Skills;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class SkillService : ComponentService, ISkillService
    {
        public T EquipSkill<T>(Transform instance) where T : SkillBase, new()
        {
            return Equip<T>(instance);
        }
    }
}
