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
        /// Scan texture2D and converts into zones with freeform lights - and adds utilities as children of zones
        /// </summary>
        /// <param name="zoneMap"></param>
        /// <param name="utilitiesMap"></param>
        void GenerateZones(Texture2D zoneMap, Texture2D utilitiesMap, Texture2D structureMap);
    }
}