using UnityEngine;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.ServiceLocator.Services.Impl;

namespace ZepLink.RiceNinja.ServiceLocator
{
    public static class Initializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            var sl = ServiceFinder.Instance;

            if (sl.IsRegistered<ICharacterService>())
                return;

            sl.Register<ICharacterService>(new CharacterService());
            sl.Register<ISkillService>(new SkillService());
            sl.Register<IAnimationService>(new AnimationService());
            var coroutineService = sl.Register<ICoroutineService>(new CoroutineService());
            var audioService = sl.Register<IAudioService>(new AudioService(coroutineService));
            var cameraService = sl.Register<ICameraService>(new CameraService());
            sl.Register<ILightService>(new LightService(coroutineService, cameraService));
            var zoneService = sl.Register<IZoneService>(new ZoneService(cameraService));
            var timeService = sl.Register<ITimeService>(new TimeService(audioService));
            var spawnService = sl.Register<ISpawnService>(new SpawnService(timeService, cameraService, zoneService));
            var touchService = sl.Register<ITouchService>(new TouchService(cameraService));
            var brushService = sl.Register<IBrushService>(new BrushService());
            var tileService = sl.Register<ITileService>(new TileService());
            var utilitiesService = sl.Register<IUtilitiesService>(new UtilitiesService(brushService, tileService, zoneService));
            sl.Register<ITutorialService>(new TutorialService(touchService, cameraService, coroutineService));
            sl.Register<IParallaxService>(new ParallaxService(cameraService));
            var scenesService = sl.Register<IScenesService>(new ScenesService(spawnService, audioService, cameraService, touchService, utilitiesService, coroutineService));
            sl.Register<IPortalService>(new PortalService(cameraService, zoneService, scenesService, coroutineService));

            // Register last
            sl.Register<IDefaultPoolService>(new DefaultPoolService());

            coroutineService.StartCoroutine(scenesService.InitialLoad());
        }
    }
}