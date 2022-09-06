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
        /// </summary>
        /// <param name="tile"></param>
        void SetTile(Vector3Int coords, TileBase tile);

        /// <summary>
        /// Generate shadows for tilemap
        /// </summary>
        void GenerateShadows();
    }
}