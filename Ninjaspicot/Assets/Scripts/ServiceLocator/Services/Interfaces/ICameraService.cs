using ZepLink.RiceNinja.Dynamics.Cameras;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ICameraService : ICollectionService<ICamera>
    {   
        /// <summary>
        /// Main game camera
        /// </summary>
        MainCamera MainCamera { get; }

        /// <summary>
        /// UI camera
        /// </summary>
        UICamera UiCamera { get; }
    }
}