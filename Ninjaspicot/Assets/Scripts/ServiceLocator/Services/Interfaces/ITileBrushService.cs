using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Map;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ITileBrushService : IGameService
    {
        /// <summary>
        /// Get first brush with given brushType
        /// </summary>
        /// <param name="brushType"></param>
        /// <returns></returns>
        TileBrush FindByBrushType(BrushType brushType);

        /// <summary>
        /// Get first brush with given color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        TileBrush FindByColor(Color color);
    }
}