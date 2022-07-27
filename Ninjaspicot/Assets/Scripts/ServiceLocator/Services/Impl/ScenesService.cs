using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.Manageables.Scenes;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class ScenesService : CoroutineService<SceneInfos>, IScenesService
    {
        [SerializeField] private int _startScene;
        [SerializeField] private int _startCheckPoint;
        [SerializeField] private AudioClip[] _sceneAudios;

        public bool IsSceneLoading { get; private set; }
        public SceneInfos CurrentScene { get; private set; }

        private Coroutine _volumeDown;
        private ISpawnService _spawnService;
        private IAudioService _audioService;

        public ScenesService(ISpawnService spawnService, IAudioService audioService)
        {
            _spawnService = spawnService;
            _audioService = audioService;
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            _spawnService = ServiceFinder.Instance.Get<ISpawnService>();

            if (_startScene < 2)
            {
                LoadLobby();
            }
            else
            {
                LoadById(_startScene);
            }

            _spawnService.InitActiveSceneSpawns(_startCheckPoint);
        }

        public void LoadLobby()
        {
            var lobby = FindByName("Lobby");
            SceneManager.LoadScene(lobby.Name);
            SwitchAudio(1);
            lobby.Loaded = true;

            // Wake lobby wakeables
            BaseUtils.FindObjectsOfTypeInScene<ISceneryWakeable>("Lobby").ForEach(w => w.Wake());
        }

        public void LoadById(int sceneId)
        {
            SceneManager.LoadScene(sceneId);
            SwitchAudio(sceneId);
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

            while (operation.progress < .9f || _volumeDown != null)
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
                sceneInfos.Loaded = true;
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
            scene.Loaded = false;
        }

        private SceneInfos GetSceneByPortalId(int portalId)
        {
            var id = int.Parse(portalId.ToString().Substring(0, 2));
            return FindById(id);
        }
    }
}