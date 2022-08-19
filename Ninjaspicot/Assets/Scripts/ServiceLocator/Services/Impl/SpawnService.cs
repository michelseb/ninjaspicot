using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class SpawnService : PoolService<CheckPoint>, ISpawnService
    {
        public override string Name => "SpawnService";

        private readonly ITimeService _timeService;
        private readonly ICameraService _cameraService;
        private readonly IZoneService _zoneService;
        private Vector3 _spawnPosition => _initialized ? new Vector3(_currentSpawn.Transform.position.x, _currentSpawn.Transform.position.y, -5) : Vector3.zero;
        private CheckPoint _currentSpawn;
        private bool _initialized => !BaseUtils.IsNull(_currentSpawn);

        public SpawnService(ITimeService timeService, ICameraService cameraService, IZoneService zoneService)
        {
            _timeService = timeService;
            _cameraService = cameraService;
            _zoneService = zoneService;
        }

        public void SpawnAtLastSpawningPosition(ISpawnable spawnable)
        {
            if (!_initialized)
                return;

            spawnable.InitSpawn();
            _timeService.SetNormalTime();
            _cameraService.MainCamera.Zoom(ZoomType.Init);
            _zoneService.SetZone(_currentSpawn.Zone.Id);

            spawnable.Transform.position = _spawnPosition;
        }

        public void SpawnAtBeginning(ISpawnable spawnable)
        {
            throw new System.NotImplementedException();
        }

        public void InitActiveSceneSpawns()
        {
            SetLatestSpawn(Collection.FirstOrDefault(x => x is StartingPoint) ?? Collection.FirstOrDefault());
        }

        public void SetLatestSpawn(CheckPoint checkPoint)
        {
            if (BaseUtils.IsNull(checkPoint) || checkPoint.Attained)
                return;

            _currentSpawn = checkPoint;
            checkPoint.Attain();
        }
    }
}