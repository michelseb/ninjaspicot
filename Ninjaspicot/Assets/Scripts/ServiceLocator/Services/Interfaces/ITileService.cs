using UnityEngine;
using UnityEngine.Tilemaps;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ITileService : IGameService
    {
        /// <summary>
        /// Game tilemap
        /// </summary>
        Tilemap Tilemap { get; }

        /// <summary>
        /// Shadow caster
        /// </summary>
        ShadowCaster Caster { get; }

        /// <summary>
        /// Sets tile on tilemap
        /// If not isAttachable then add trigger to detach characters from platform
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="tile"></param>
        /// <param name="isAttachable"></param>
        void SetTile(Vector3Int coords, TileBase tile, bool isAttachable);

        /// <summary>
        /// Generate shadows for tilemap
        /// </summary>
        void GenerateShadows();
    }
}