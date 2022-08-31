using UnityEngine;
using UnityEngine.Tilemaps;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ITileService : IGameService
    {
        /// <summary>
        /// Game tilemap
        /// </summary>
        Tilemap Tilemap { get; }

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