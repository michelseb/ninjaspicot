using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class ScriptableObjectService<T, U> : CollectionService<T, U>, IScriptableObjectService<T, U> where U : ScriptableObject, IManageable<T>
    {
        public abstract string ObjectsPath { get; }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            if (string.IsNullOrEmpty(ObjectsPath))
            {
                Debug.LogError($"ObjectsPath wasn't set for service {GetType()}");
                return;
            }

            Resources
                .LoadAll(ObjectsPath)
                .Cast<U>()
                .ToList()
                .ForEach(s => Add(s));
        }
    }
}
