using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
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

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;

        public virtual Transform GetParent(Transform parentZone)
        {

            if (BaseUtils.IsNull(parentZone))
            {
                return GameObject.Find(ParentName)?.transform ?? new GameObject(ParentName).transform;
            }
            else
            {
                return parentZone.Find(ParentName)?.transform ?? new GameObject(ParentName).transform;
            }
        }
    }
}
