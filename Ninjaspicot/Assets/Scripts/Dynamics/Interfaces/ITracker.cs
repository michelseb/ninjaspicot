using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface ITracker : IDynamic
    {
        Vector3 NormalVector { get; }
    }
}