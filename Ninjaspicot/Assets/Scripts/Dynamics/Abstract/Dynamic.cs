using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Abstract
{
    public abstract class Dynamic : MonoBehaviour, IDynamic
    {
        private int _id;
        public virtual int Id { get { if (_id == default) _id = gameObject.GetInstanceID(); return _id; } }

        public virtual string Name => gameObject.name;

        public virtual string ParentName => $"{GetType().Name}s";

        private Transform _transform;
        public virtual Transform Transform { get { if (BaseUtils.IsNull(_transform)) _transform = transform; return _transform; } }

        private Transform _parent;
        public virtual Transform Parent
        {
            get
            {
                if (BaseUtils.IsNull(_parent))
                {
                    _parent = GameObject.Find(ParentName)?.transform ?? new GameObject(ParentName).transform;
                }

                return _parent;
            }
        }


        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}
