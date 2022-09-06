using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZepLink.RiceNinja.Dynamics.Characters.Ninjas.MainCharacter;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities;
using ZepLink.RiceNinja.Helpers;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Scenes;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class ScenesService : ScriptableObjectService<int, SceneInfos>, IScenesService
    {
        public bool IsSceneLoading { get; private set; }
        public SceneInfos CurrentScene { get; private set; }

        public override string ObjectsPath => "Scenes";

        private readonly ISpawnService _spawnService;
        private readonly IAudioService _audioService;
        private readonly ICameraService _cameraService;
        private readonly ITouchService _touchService;
        private readonly IUtilitiesService _utilitiesService;
        private readonly ICoroutineService _coroutineService;

        public ScenesService(ISpawnService spawnService, IAudioService audioService, ICameraService cameraService,
            ITouchService touchService, IUtilitiesService utilitiesService, ICoroutineService coroutineService)
        {
            _spawnService = spawnService;
            _audioService = audioService;
            _cameraService = cameraService;
            _touchService = touchService;
            _utilitiesService = utilitiesService;
            _coroutineService = coroutineService;
        }

        public IEnumerator InitialLoad(int sceneIndex = 0)
        {
            if (sceneIndex < 2)
            {
                yield return _coroutineService.StartCoroutine(LoadLobby());
            }
            else
            {
                yield return _coroutineService.StartCoroutine(LoadById(sceneIndex, true));
                //yield return _coroutineService.StartCoroutine(LoadById(sceneIndex, true), out _);
            }

            _spawnService.InitActiveSceneSpawns();

            var hero = PoolHelper.Pool<Hero>();

            _spawnService.SpawnAtLastSpawningPosition(hero);
            _cameraService.MainCamera.SetTracker(hero);
            _touchService.SetControllable(hero);
        }

        public IEnumerator LoadLobby()
        {
            var lobby = FindByName("Lobby");

            _coroutineService.StartCoroutine(LoadByName(lobby.name, false), out Guid load);

            while (_coroutineService.IsCoroutineRunning(load))
                yield return null;

            // Wake lobby wakeables
            BaseUtils.FindObjectsOfTypeInScene<ISceneryWakeable>("Lobby").ForEach(w => w.Wake());
        }

        private IEnumerator LoadByName(string sceneName, bool unloadPrevious)
        {
            //SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

            var scene = FindByName(sceneName);

            if (scene == null)
                yield break;

            _coroutineService.StartCoroutine(LoadAdditional(scene), out Guid load);

            while (_coroutineService.IsCoroutineRunning(load))
                yield return null;

            Enable(scene);

            _utilitiesService.Generate(CurrentScene.ZoneMap, CurrentScene.UtilitiesMap, CurrentScene.StructureMap);
            
            PoolHelper.Pool<Background>().SetImage(CurrentScene.Background);
        }

        public IEnumerator LoadById(int sceneId, bool unloadPrevious)
        {
            var scene = FindById(sceneId);
            yield return _coroutineService.StartCoroutine(LoadByName(scene.Name, unloadPrevious));
        }

        private void SwitchAudio(int sceneId)
        {
            var scene = FindById(sceneId);

            _audioService.PauseGlobal();
            _audioService.PlayGlobal(scene.Audio, 1);
        }

        private IEnumerator LoadAdditional(SceneInfos scene)
        {
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

            IsSceneLoading = false;
        }

        private void Enable(SceneInfos sceneInfos)
        {
            var sceneToLoad = SceneManager.GetSceneByName(sceneInfos.Name);

            if (!sceneToLoad.IsValid())
                return;

            SceneManager.MoveGameObjectToScene(_cameraService.CamerasContainer, sceneToLoad);
            SceneManager.SetActiveScene(sceneToLoad);
            _spawnService.InitActiveSceneSpawns();
            CurrentScene = sceneInfos;
            SwitchAudio(sceneToLoad.buildIndex);
            sceneInfos.Load();
        }

        public void StartLoadingByPortalId(int portalId)
        {
            var scene = GetSceneByPortalId(portalId);

            StartLoading(scene);
        }

        public void StartLoadingById(int sceneId)
        {
            var scene = FindById(sceneId);

            StartLoading(scene);
        }

        private void StartLoading(SceneInfos scene)
        {
            if (IsSceneLoading || scene == null)
                return;

            _coroutineService.StartCoroutine(LoadAdditional(scene));
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