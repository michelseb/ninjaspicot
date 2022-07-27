namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ILightService : IGameService
    {
        /// <summary>
        /// Activate or deactivate all lights
        /// </summary>
        /// <param name="active"></param>
        void SetLightsActivation(bool active);
    }
}