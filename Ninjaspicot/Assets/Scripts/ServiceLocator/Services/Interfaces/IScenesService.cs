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
        /// Load lobby scene
        /// </summary>
        void LoadLobby();

        /// <summary>
        /// Load scene with given id
        /// </summary>
        /// <param name="sceneId"></param>
        void LoadById(int sceneId);

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