using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Manageables;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class CollectionService<T> : ICollectionService<T> where T : IManageable
    {
        public IDictionary<int, T> InstancesDictionary { get; } = new Dictionary<int, T>();
        public virtual IList<T> Collection => InstancesDictionary.Values.ToList();

        public virtual GameObject ServiceObject { get; protected set; }

        public virtual void Init(Transform parent)
        {
            ServiceObject = new GameObject(GetType().Name);
            ServiceObject.transform.SetParent(parent);
        }

        public void Add(T instance)
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

        public void Remove(int id)
        {
            if (!InstancesDictionary.ContainsKey(id))
            {
                Debug.LogError($"trying to remove non-existing instance of {GetType()} to collection");
                return;
            }

            InstancesDictionary.Remove(id);
        }

        public T FindById(int id)
        {
            if (!InstancesDictionary.ContainsKey(id))
            {
                Debug.LogError($"Instance of type {GetType()} with id {id} does not exist");
                return default;
            }

            return InstancesDictionary[id];
        }

        public T FindByName(string name)
        {
            return Collection.FirstOrDefault(x => x.Name == name);
        }
    }
}
