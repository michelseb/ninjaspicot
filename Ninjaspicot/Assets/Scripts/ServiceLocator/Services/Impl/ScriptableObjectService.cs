using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Manageables;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public abstract class ScriptableObjectService<T> : CollectionService<T>, IScriptableObjectService<T> where T : ScriptableObject, IManageable
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

            var scriptables = Resources.LoadAll(ObjectsPath).Cast<T>().ToList();
            scriptables.ForEach(s => Add(s));
        }
    }
}
