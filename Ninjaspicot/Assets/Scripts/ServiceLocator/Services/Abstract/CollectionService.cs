using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class CollectionService<T, U> : ICollectionService<T, U> where U : IManageable<T>
    {
        public IDictionary<T, U> InstancesDictionary { get; } = new Dictionary<T, U>();
        public virtual IList<U> Collection => InstancesDictionary.Values.ToList();

        public virtual GameObject ServiceObject { get; protected set; }

        public virtual void Init(Transform parent)
        {
            ServiceObject = new GameObject(GetType().Name);
            ServiceObject.transform.SetParent(parent);
        }

        public void Add(U instance)
        {
            if (BaseUtils.IsNull(instance))
            {
                Debug.LogError($"trying to add null instance of {GetType()} to collection");
                return;
            }

            if (InstancesDictionary.ContainsKey(instance.Id))
            {
                Debug.LogError($"trying to add already existing instance of {GetType()} to collection");
                return;
            }

            InstancesDictionary.Add(instance.Id, instance);
        }

        public void Remove(T id)
        {
            if (!InstancesDictionary.ContainsKey(id))
            {
                Debug.LogError($"trying to remove non-existing instance of {GetType()} to collection");
                return;
            }

            InstancesDictionary.Remove(id);
        }

        public U FindById(T id)
        {
            if (!InstancesDictionary.ContainsKey(id))
            {
                Debug.LogError($"Instance of type {GetType()} with id {id} does not exist");
                return default;
            }

            return InstancesDictionary[id];
        }

        public U FindByName(string name)
        {
            return Collection.FirstOrDefault(x => x.Name == name);
        }
    }
}
