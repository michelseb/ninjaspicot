using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Portals;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IPortalService : IGameService
    {
        /// <summary>
        /// Is connection being made
        /// </summary>
        bool Connecting { get; }

        /// <summary>
        /// Initiates connection between given entrance portal and associated exit
        /// </summary>
        /// <param name="entrance"></param>
        void LaunchConnection(Portal entrance);

        /// <summary>
        /// Teleports teleportable from entrance to exit
        /// </summary>
        /// <param name="teleportable"></param>
        /// <param name="entrance"></param>
        /// <param name="exit"></param>
        void Teleport(ITeleportable teleportable, Portal entrance, Portal exit);

        /// <summary>
        /// Close zone that contains previous portal after teleporting
        /// </summary>
        /// <param name="entranceId"></param>
        void ClosePreviousScene(int entranceId);

        /// <summary>
        /// Reinit teleporting status
        /// </summary>
        /// <param name="entrance"></param>
        /// <param name="exit"></param>
        void TerminateConnection(Portal entrance, Portal exit = null);

        /// <summary>
        /// Check wether a connection is available from given entrance
        /// </summary>
        /// <param name="entranceId"></param>
        /// <returns></returns>
        bool ConnectionExists(int entranceId);
    }
}