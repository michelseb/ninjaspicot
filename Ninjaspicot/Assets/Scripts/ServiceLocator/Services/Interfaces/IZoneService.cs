using ZepLink.RiceNinja.Dynamics.Scenery.Zones;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IZoneService : ICollectionService<int, Zone>
    {
        /// <summary>
        /// Current zone
        /// </summary>
        public Zone CurrentZone { get; }

        /// <summary>
        /// Set current zone
        /// </summary>
        /// <param name="zoneId"></param>
        void SetZone(int zoneId);

        /// <summary>
        /// Open zone without closing previous one (eg for bonuses)
        /// </summary>
        /// <param name="zoneId"></param>
        void OpenExtraZone(int zoneId);

        /// <summary>
        /// Set camera behaviour depending on zone
        /// </summary>
        void UpdateCurrentZoneCameraBehavior();
    }
}