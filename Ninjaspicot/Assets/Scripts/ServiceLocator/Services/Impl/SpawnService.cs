using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class SpawnService : PoolService<CheckPoint>, ISpawnService
    {
        public override string Name => "SpawnService";

        private readonly ITimeService _timeService;
        private readonly ICameraService _cameraService;
        private Vector3 _spawnPosition;

        public SpawnService(ITimeService timeService, ICameraService cameraService)
        {
            _timeService = timeService;
            _cameraService = cameraService;
        }

        public void SpawnAtLastSpawningPosition(ISpawnable spawnable)
        {
            spawnable.InitSpawn();
            _timeService.SetNormalTime();
            _cameraService.MainCamera.Zoom(ZoomType.Init);

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

            _spawnPosition = new Vector3(checkPoint.transform.position.x, checkPoint.transform.position.y, -5);
            checkPoint.Attain();
        }
    }
}