using UnityEngine;
using ZepLink.RiceNinja.Manageables;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.ServiceLocator.Services.Impl;

namespace ZepLink.RiceNinja.ServiceLocator
{
    public static class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            var sl = ServiceFinder.Instance;

            sl.Register<ICharacterService>(new CharacterService());
            sl.Register<ILightService>(new LightService());
            sl.Register<ISkillService>(new SkillService());
            var audioService = sl.Register<IAudioService>(new AudioService());
            var coroutineService = sl.Register<ICoroutineService>(new CoroutineService<int, IntManageable>());
            var poolService = sl.Register<IPoolService>(new PoolService());
            var cameraService = sl.Register<ICameraService>(new CameraService());
            var timeService = sl.Register<ITimeService>(new TimeService(audioService));
            var spawnService = sl.Register<ISpawnService>(new SpawnService(timeService, cameraService));
            var touchService = sl.Register<ITouchService>(new TouchService(poolService, cameraService));
            var zoneService = sl.Register<IZoneService>(new ZoneService(cameraService));
            var tileBrushService = sl.Register<ITileBrushService>(new TileBrushService());
            var tileService = sl.Register<ITileService>(new TileService(tileBrushService));
            sl.Register<ITutorialService>(new TutorialService(touchService, cameraService, coroutineService));
            sl.Register<IParallaxService>(new ParallaxService(cameraService));
            var scenesService = sl.Register<IScenesService>(new ScenesService(spawnService, audioService, tileService));
            sl.Register<IPortalService>(new PortalService(cameraService, zoneService, scenesService));

            scenesService.InitialLoad();
        }
    }
}