using UnityEngine;
using ZepLink.RiceNinja.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IPoolable : IDynamic, IWakeable, IResettable
    {
        void Pool(Vector3 position, Quaternion rotation, float size = 1);
    }
}
