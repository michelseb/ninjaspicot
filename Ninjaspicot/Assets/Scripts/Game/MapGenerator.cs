using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.ServiceLocator.Services.Impl;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Managers
{
    public class MapGenerator : MonoBehaviour
    {
        private IBrushService _brushService;
        public IBrushService BrushService
        {
            get
            {
                if (BaseUtils.IsNull(_brushService))
                {
                    _brushService = new BrushService();
                    _brushService.Init(transform);
                }

                return _brushService;
            }
        }

        private ICameraService _cameraService;
        public ICameraService CameraService { get { if (BaseUtils.IsNull(_cameraService)) _cameraService = new CameraService(); return _cameraService; } }

        private ITileService _tileService;
        public ITileService TileService { get { if (BaseUtils.IsNull(_tileService)) _tileService = new TileService(); return _tileService; } }

        private IZoneService _zoneService;
        public IZoneService ZoneService { get { if (BaseUtils.IsNull(_zoneService)) _zoneService = new ZoneService(CameraService); return _zoneService; } }

        private IMapService _mapService;
        public IMapService MapService { get { if (BaseUtils.IsNull(_mapService)) _mapService = new MapService(BrushService, TileService); return _mapService; } }

        //private ILightMapService _lightMapService;
        //public ILightMapService LightMapService { get { if (BaseUtils.IsNull(_lightMapService)) _lightMapService = new LightMapService(BrushService, TileService); return _lightMapService; } }

        private IUtilitiesService _utilitiesService;
        public IUtilitiesService UtilitiesService { get { if (BaseUtils.IsNull(_utilitiesService)) _utilitiesService = new UtilitiesService(BrushService, TileService, ZoneService); return _utilitiesService; } }

        private IAudioService _audioService;
        public IAudioService AudioService { get { if (BaseUtils.IsNull(_audioService)) _audioService = new AudioService(CoroutineService); return _audioService; } }

        private ITimeService _timeService;
        public ITimeService TimeService { get { if (BaseUtils.IsNull(_timeService)) _timeService = new TimeService(AudioService); return _timeService; } }

        private ISpawnService _spawnService;
        public ISpawnService SpawnService { get { if (BaseUtils.IsNull(_spawnService)) _spawnService = new SpawnService(TimeService, CameraService, ZoneService); return _spawnService; } }

        private ITouchService _touchService;
        public ITouchService TouchService { get { if (BaseUtils.IsNull(_touchService)) _touchService = new TouchService(CameraService); return _touchService; } }

        private ICoroutineService _coroutineService;
        public ICoroutineService CoroutineService { get { if (BaseUtils.IsNull(_coroutineService)) _coroutineService = new CoroutineService(); return _coroutineService; } }

        private IScenesService _scenesService;
        public IScenesService ScenesService
        {
            get
            {
                if (BaseUtils.IsNull(_scenesService))
                {
                    _scenesService = new ScenesService(SpawnService, AudioService, CameraService, TouchService, UtilitiesService, CoroutineService);
                    _scenesService.Init(transform);
                }

                return _scenesService;
            }
        }

        public void Generate()
        {
            var objectScene = gameObject.scene;

            var currentScene = ScenesService.FindByName(objectScene.name);

            if (currentScene == null)
                return;

            SceneManager.SetActiveScene(objectScene);

            // Destroy previous
            var id = gameObject.GetInstanceID();
            FindObjectsOfType<GameObject>().Where(o => o.scene.buildIndex == objectScene.buildIndex && o.GetInstanceID() != id).ToList().ForEach(x => DestroyImmediate(x));

            MapService.Generate(currentScene.StructureMap);

            // Cleanup
            foreach (Transform child in transform)
            {
                if (child.GetInstanceID() == id)
                    continue;

                DestroyImmediate(child);
            }

            #if UNITY_EDITOR
            EditorUtility.SetDirty(TileService.Tilemap);
            #endif

            SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
        }

        public void GenerateShadowCasters()
        {
            TileService.GenerateShadows();

            #if UNITY_EDITOR
            EditorUtility.SetDirty(TileService.Caster);
            #endif
        }
    }
}
