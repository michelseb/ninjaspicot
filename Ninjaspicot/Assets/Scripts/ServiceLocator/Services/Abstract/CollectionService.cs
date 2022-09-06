using System.Collections.Generic;
using System.Linq;
using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Abstract
{
    public abstract class CollectionService<T, U> : GameService, ICollectionService<T, U> where U : IManageable<T>
    {
        public IDictionary<T, U> InstancesDictionary { get; } = new Dictionary<T, U>();
        public virtual IList<U> Collection => InstancesDictionary.Values.ToList();

        public virtual void Add(U instance)
        {
            if (BaseUtils.IsNull(instance))
            {
                Log($"trying to add null instance of {GetType()} to collection");
                return;
            }

            if (InstancesDictionary.ContainsKey(instance.Id))
            {
                Log($"trying to add already existing instance of {GetType()} to collection");
                return;
            }

            InstancesDictionary.Add(instance.Id, instance);
        }

        public virtual void Remove(T id)
        {
            if (!InstancesDictionary.ContainsKey(id))
            {
                Log($"trying to remove non-existing instance of {GetType()} to collection");
                return;
            }

            InstancesDictionary.Remove(id);
        }

        public U FindById(T id)
        {
            if (!InstancesDictionary.ContainsKey(id))
            {
                Log($"Instance of type {typeof(U)} with id {id} does not exist");
                return default;
            }

            return InstancesDictionary[id];
        }

        public U FindByName(string name)
        {
            return Collection.FirstOrDefault(x => x.Name == name);
        }

        public void AddBase(IManageable instance)
        {
            Add((U)instance);
        }

        public void RemoveBase(IManageable id)
        {
            Remove((T)id);
        }
    }
}
