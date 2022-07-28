using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ITileService : IGameService
    {
        /// <summary>
        /// Scan texture2D and converts into level with given brush
        /// </summary>
        /// <param name="map"></param>
        /// <param name="brushType"></param>
        void Generate(Texture2D map, BrushType brushType);
    }
}