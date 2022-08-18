using UnityEngine;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IMapService : IGameService
    {
        /// <summary>
        /// Scan texture2D and converts into level
        /// </summary>
        /// <param name="map"></param>
        void Generate(Texture2D map);

        /// <summary>
        /// Scan texture2D and converts into freeform lights
        /// </summary>
        /// <param name="lightMap"></param>
        void GenerateLights(Texture2D lightMap);
    }
}