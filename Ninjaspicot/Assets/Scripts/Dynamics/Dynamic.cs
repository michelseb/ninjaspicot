using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics
{
    public abstract class Dynamic : MonoBehaviour, IDynamic
    {
        private int _id;
        public virtual int Id { get { if (_id == default) _id = gameObject.GetInstanceID(); return _id; } }

        public virtual string Name => "Dynamic";

        private Transform _transform;
        public virtual Transform Transform { get { if (BaseUtils.IsNull(_transform)) _transform = transform; return _transform; } }

        private ServiceFinder _serviceFinder;
        public ServiceFinder ServiceFinder { get { if (_serviceFinder == null) _serviceFinder = ServiceFinder.Instance; return _serviceFinder; } }
    }
}
