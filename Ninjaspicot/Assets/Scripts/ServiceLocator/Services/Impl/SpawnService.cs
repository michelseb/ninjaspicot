using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class SpawnService : CollectionService<CheckPoint>, ISpawnService
    {
        private readonly ITimeService _timeService;
        private readonly ICameraService _cameraService;
        private Vector3 _spawnPosition;

        public override IList<CheckPoint> Collection => InstancesDictionary.Select(d => d.Value).OrderBy(x => x.Order).ToArray();

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

        public void InitActiveSceneSpawns(int checkPoint = 0)
        {
            SetLatestSpawn(Collection.FirstOrDefault(x => x.Order == checkPoint) ?? Collection[0]);
        }

        public void SetLatestSpawn(CheckPoint checkPoint)
        {
            if (checkPoint.Attained)
                return;

            _spawnPosition = new Vector3(checkPoint.transform.position.x, checkPoint.transform.position.y, -5);
            checkPoint.Attain();
        }
    }
}