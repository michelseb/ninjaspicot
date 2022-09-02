﻿using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Abstract;
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

            if (sl.IsRegistered<ICharacterService>())
                return;

            sl.Register<ICharacterService>(new CharacterService());
            sl.Register<ISkillService>(new SkillService());
            sl.Register<IAnimationService>(new AnimationService());
            var audioService = sl.Register<IAudioService>(new AudioService());
            var coroutineService = sl.Register<ICoroutineService>(new CoroutineService<Guid, GuidManageable>());
            sl.Register<ILightService>(new LightService(coroutineService));
            var cameraService = sl.Register<ICameraService>(new CameraService());
            var zoneService = sl.Register<IZoneService>(new ZoneService(cameraService));
            var timeService = sl.Register<ITimeService>(new TimeService(audioService));
            var spawnService = sl.Register<ISpawnService>(new SpawnService(timeService, cameraService, zoneService));
            var touchService = sl.Register<ITouchService>(new TouchService(cameraService));
            var brushService = sl.Register<IBrushService>(new BrushService());
            var tileService = sl.Register<ITileService>(new TileService());
            var mapService = sl.Register<IMapService>(new MapService(brushService, tileService, zoneService));
            sl.Register<ITutorialService>(new TutorialService(touchService, cameraService, coroutineService));
            sl.Register<IParallaxService>(new ParallaxService(cameraService));
            var scenesService = sl.Register<IScenesService>(new ScenesService(spawnService, audioService, mapService, cameraService, touchService));
            sl.Register<IPortalService>(new PortalService(cameraService, zoneService, scenesService));

            // Register last
            sl.Register<IDefaultPoolService>(new DefaultPoolService());

            scenesService.InitialLoad();
        }
    }
}