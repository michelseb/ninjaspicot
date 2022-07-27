using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Cameras
{
    public interface ICamera : IDynamic
    {
        /// <summary>
        /// Camera component
        /// </summary>
        Camera Camera { get; }

        /// <summary>
        /// Orthographic size
        /// </summary>
        float Size { get; }
    }
}