using ZepLink.RiceNinja.Dynamics.Scenery.Zones;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class ZoneService : CollectionService<int, Zone>, IZoneService
    {
        public Zone CurrentZone { get; private set; }

        private long _currentZoneId;

        private readonly ICameraService _cameraService;

        public ZoneService(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public void SetZone(int zoneId)
        {
            if (zoneId == _currentZoneId)
                return;

            _currentZoneId = zoneId;

            var zone = FindById(zoneId);

            if (!zone.Initialized)
            {
                zone.Init();
            }

            if (CurrentZone)
            {
                CurrentZone.CloseForever();
            }
            else
            {
                for (int i = 0; i < Collection.Count; i++)
                {
                    var z = Collection[i];
                    
                    if (z == zone)
                        continue;

                    if (!z)
                    {
                        Collection.RemoveAt(i);
                        continue;
                    }

                    if (!z.Initialized)
                    {
                        z.Init();
                    }

                    z.Close();
                }
            }

            CurrentZone = zone;
            CurrentZone.Open();

            UpdateCurrentZoneCameraBehavior();
        }

        public void OpenExtraZone(int zoneId)
        {
            var zone = FindById(zoneId);

            zone.Open();

            if (zone.Center.HasValue)
            {
                _cameraService.MainCamera.SetCenterMode(zone.Center.Value);
            }
            else
            {
                _cameraService.MainCamera.SetFollowMode();
            }
        }

        public void UpdateCurrentZoneCameraBehavior()
        {
            if (BaseUtils.IsNull(CurrentZone))
                return;

            if (CurrentZone.Center.HasValue)
            {
                _cameraService.MainCamera.SetCenterMode(CurrentZone.Center.Value);
            }
            else
            {
                _cameraService.MainCamera.SetFollowMode();
            }
        }
    }
}