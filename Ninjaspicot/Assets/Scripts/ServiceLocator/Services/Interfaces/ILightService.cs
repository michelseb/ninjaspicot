using ZepLink.RiceNinja.Dynamics.Effects.Lights;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ILightService : IPoolService<LightEffect>
    {
        /// <summary>
        /// Activate or deactivate all lights
        /// </summary>
        /// <param name="active"></param>
        void SetLightsActivation(bool active);
    }
}