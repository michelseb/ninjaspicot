using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class DefaultPoolService : PoolService<IPoolable>, IDefaultPoolService
    {
        protected override string ModelPath => "Poolables/Default";

        private bool _upToDate;
        private IDictionary<Type, List<IPoolable>> _poolables;
        public IDictionary<Type, List<IPoolable>> Poolables
        {
            get
            {
                if (!_upToDate)
                {
                    _poolables = Collection.GroupBy(p => p.GetType()).ToDictionary(x => x.Key, x => x.ToList());
                    _upToDate = true;
                }

                return _poolables;
            }
        }

        public override void Add(IPoolable instance)
        {
            base.Add(instance);
            _upToDate = false;
        }

        public override void Remove(int id)
        {
            base.Remove(id);
            _upToDate = false;
        }

        public override IPoolable PoolAt(Vector3 position, Quaternion rotation, float size, string modelName = null, Transform zone = default)
        {
            return base.PoolAt(position, rotation, size, modelName, zone);
        }

        public IPoolable PoolAt(Vector3 position, Quaternion rotation, float size, Type poolableType, string modelName = null, Transform zone = default)
        {
            if (!Poolables.ContainsKey(poolableType) || !Poolables[poolableType].Any())
                return Create(_models.Values.FirstOrDefault(x => IsModelValid(x.Name, modelName, x.GetType(), poolableType)), position, rotation, zone);

            var poolList = Poolables[poolableType];
            var poolable = poolList.FirstOrDefault(x => !x.Transform.gameObject.activeSelf);

            if (poolable == null)
                return Create(poolList.FirstOrDefault(x => IsModelValid(x.Name, modelName, x.GetType(), poolableType)), position, rotation, zone);

            poolable.Wake();
            poolable.Pool(position, rotation, size);

            return poolable;
        }

        protected bool IsModelValid(string name, string modelName, Type type, Type modelType)
        {
            if (!IsModelValid(name, modelName))
                return false;

            return type == modelType;
        }
    }
}
