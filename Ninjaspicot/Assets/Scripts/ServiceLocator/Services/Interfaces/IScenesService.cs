using System.Collections;
using ZepLink.RiceNinja.Manageables.Scenes;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IScenesService : IScriptableObjectService<int, SceneInfos>
    {
        /// <summary>
        /// Is a scene being loaded
        /// </summary>
        /// <returns></returns>
        bool IsSceneLoading { get; }

        /// <summary>
        /// First load
        /// </summary>
        /// <param name="sceneIndex"></param>
        IEnumerator InitialLoad(int sceneIndex = 0);

        /// <summary>
        /// Load lobby scene
        /// </summary>
        IEnumerator LoadLobby();

        /// <summary>
        /// Load scene with given id
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="unloadPrevious"></param>
        IEnumerator LoadById(int sceneId, bool unloadPrevious);

        /// <summary>
        /// Unloads scene with given id
        /// </summary>
        /// <param name="sceneId"></param>
        void Unload(int sceneId);

        /// <summary>
        /// Loads additional scene with given Id
        /// </summary>
        /// <param name="sceneId"></param>
        void StartLoadingById(int sceneId);

        /// <summary>
        /// Loads scene containing given portal
        /// </summary>
        /// <param name="portalId"></param>
        void StartLoadingByPortalId(int portalId);
    }
}