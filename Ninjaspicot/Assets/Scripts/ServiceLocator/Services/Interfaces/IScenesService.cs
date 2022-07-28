namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IScenesService : IGameService
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
        void InitialLoad(int sceneIndex = 0);

        /// <summary>
        /// Load lobby scene
        /// </summary>
        void LoadLobby();

        /// <summary>
        /// Load scene with given id
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="unloadPrevious"></param>
        void LoadById(int sceneId, bool unloadPrevious);

        /// <summary>
        /// Load scene with given name
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="unloadPrevious"></param>
        void LoadByName(string sceneName, bool unloadPrevious);

        /// <summary>
        /// Unloads scene with given id
        /// </summary>
        /// <param name="sceneId"></param>
        void Unload(int sceneId);

        /// <summary>
        /// Loads scene containing given portal
        /// </summary>
        /// <param name="portalId"></param>
        void StartLoadingByPortalId(int portalId);
    }
}