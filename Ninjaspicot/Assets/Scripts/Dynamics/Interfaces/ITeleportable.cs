using UnityEngine;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface ITeleportable : IDynamic
    {
        /// <summary>
        /// Renderers to mask
        /// </summary>
        SpriteRenderer[] Renderers { get; }

        /// <summary>
        /// Teleportable rigidbody
        /// </summary>
        Rigidbody2D Rigidbody { get; }

        /// <summary>
        /// Reference of the initial layer mask
        /// </summary>
        LayerMask InitialLayerMask { get; }

        /// <summary>
        /// Is teleportable ready to teleport
        /// </summary>
        bool CanTeleport { get; }

        /// <summary>
        /// Sets layer (useful when teleporting)
        /// </summary>
        /// <param name="layer"></param>
        void SetLayer(int layer);

        /// <summary>
        /// Disappear into teleporter
        /// </summary>
        void Disappear();

        /// <summary>
        /// Appear from teleporter
        /// </summary>
        void Appear();

        /// <summary>
        /// Prepares teleportable to be teleported
        /// </summary>
        void SetTeleportState();
    }
}