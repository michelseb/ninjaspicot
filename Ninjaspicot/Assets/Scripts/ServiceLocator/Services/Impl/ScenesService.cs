using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Scenes;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class ScenesService : ScriptableObjectService<int, SceneInfos>, ICoroutineService, IScenesService
    {
        public bool IsSceneLoading { get; private set; }
        public SceneInfos CurrentScene { get; private set; }

        public override string ObjectsPath => "Scenes";

        public IDictionary<Guid, Coroutine> RunningRoutines { get; } = new Dictionary<Guid, Coroutine>();

        public MonoBehaviour CoroutineServiceBehaviour { get; private set; }

        private readonly ISpawnService _spawnService;
        private readonly IAudioService _audioService;
        private readonly IMapService _mapService;
        private readonly ICameraService _cameraService;
        private readonly ITouchService _touchService;

        public ScenesService(ISpawnService spawnService, IAudioService audioService, IMapService mapService, ICameraService cameraService, ITouchService touchService)
        {
            _spawnService = spawnService;
            _audioService = audioService;
            _mapService = mapService;
            _cameraService = cameraService;
            _touchService = touchService;
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            CoroutineServiceBehaviour = ServiceObject.AddComponent<ServiceBehaviour>();
        }

        public void InitialLoad(int sceneIndex = 0)
        {
            if (sceneIndex < 2)
            {
                LoadLobby();
            }
            else
            {
                LoadById(sceneIndex, true);
            }

            _spawnService.InitActiveSceneSpawns();
            
            var hero = PoolHelper.Pool<Hero>();

            _cameraService.MainCamera.SetTracker(hero);
            _touchService.SetControllable(hero);
            _spawnService.SpawnAtLastSpawningPosition(hero);
        }

        public void LoadLobby()
        {
            var lobby = FindByName("Lobby");
            LoadByName(lobby.name, false);
            SwitchAudio(0);
            lobby.Load();

            // Wake lobby wakeables
            BaseUtils.FindObjectsOfTypeInScene<ISceneryWakeable>("Lobby").ForEach(w => w.Wake());
        }

        public void LoadByName(string sceneName, bool unloadPrevious)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            CurrentScene = FindByName(sceneName);
            SwitchAudio(CurrentScene.Id);

            _mapService.Generate(CurrentScene.StructureMap);

            Physics2D.SyncTransforms();
            Physics2D.simulationMode = SimulationMode2D.Script;
            Physics2D.Simulate(Time.fixedDeltaTime);
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

            Physics.SyncTransforms();
            Physics.autoSimulation = false;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.autoSimulation = true;

            _mapService.GenerateZones(CurrentScene.ZoneMap, CurrentScene.UtilitiesMap, CurrentScene.StructureMap);
        }

        public void LoadById(int sceneId, bool unloadPrevious)
        {
            var scene = FindById(sceneId);
            LoadByName(scene.Name, unloadPrevious);
        }

        private void SwitchAudio(int sceneId)
        {
            var scene = FindById(sceneId);

            _audioService.PauseGlobal();
            _audioService.PlayGlobal(scene.Audio, 1);
        }

        private IEnumerator LoadAdditional(int portalId)
        {
            var scene = GetSceneByPortalId(portalId);

            if (scene == null || scene.Loaded)
                yield break;

            _audioService.SetGlobalVolumeProgressive(0, 1);
            var operation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
            operation.allowSceneActivation = false;

            while (operation.progress < .9f)
                yield return null;

            operation.allowSceneActivation = true;

            while (!operation.isDone)
                yield return null;

            Enable(scene);
            IsSceneLoading = false;
        }

        private void Enable(SceneInfos sceneInfos)
        {
            var sceneToLoad = SceneManager.GetSceneByName(sceneInfos.Name);
            if (sceneToLoad.IsValid())
            {
                //SceneManager.MoveGameObjectToScene(Hero.Instance.gameObject, sceneToLoad);
                SceneManager.SetActiveScene(sceneToLoad);
                _spawnService.InitActiveSceneSpawns();
                CurrentScene = sceneInfos;
                SwitchAudio(sceneToLoad.buildIndex);
                sceneInfos.Load();
            }
        }

        public void StartLoadingByPortalId(int portalId)
        {
            if (IsSceneLoading)
                return;

            CoroutineServiceBehaviour.StartCoroutine(LoadAdditional(portalId));
            IsSceneLoading = true;
        }

        public void Unload(int sceneId)
        {
            var scene = FindById(sceneId);

            if (scene == null || !scene.Loaded)
                return;

            SceneManager.UnloadSceneAsync(scene.Name);
            scene.Unload();
        }

        private SceneInfos GetSceneByPortalId(int portalId)
        {
            var id = int.Parse(portalId.ToString().Substring(0, 2));
            return FindById(id);
        }
    }
}