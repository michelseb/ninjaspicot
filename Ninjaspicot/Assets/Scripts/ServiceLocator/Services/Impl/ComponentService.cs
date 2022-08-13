using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class ComponentService : CollectionService<int, IComponent>, IComponentService
    {
        public T Equip<T>(Transform instance) where T : class, IComponent
        {
            if (instance.gameObject.TryGetComponent(out T component))
            {
                Debug.LogWarning($"Service {GetType()} trying to add already existing component {component.Name} to instance {instance.name}");
                return component;
            }


            var result = instance.gameObject.AddComponent(typeof(T)) as T;
            Add(result);

            return result;
        }
    }
}
