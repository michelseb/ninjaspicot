using ZepLink.RiceNinja.Dynamics.Effects.Lights;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ILightService : IPoolService<LightEffect>
    {
        /// <summary>
        /// Current ambiant light
        /// </summary>
        AmbiantLight CurrentAmbiant { get; }

        /// <summary>
        /// Activate or deactivate all lights
        /// </summary>
        /// <param name="active"></param>
        void SetLightsActivation(bool active);
        
        /// <summary>
        /// Makes current ambiant light brighter
        /// </summary>
        void BrightenAmbiant();

        /// <summary>
        /// Makes current ambiant light dimmer
        /// </summary>
        void DimmAmbiant();

        /// <summary>
        /// Makes ChromaticAberration effect flash for .3 seconds
        /// </summary>
        /// <param name="duration"></param>
        void ChromaBlast();

        /// <summary>
        /// Sets current ambiant light
        /// </summary>
        /// <param name="ambiantLight"></param>
        void SetAmbiant(AmbiantLight ambiantLight);
    }
}