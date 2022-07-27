using ZepLink.RiceNinja.Dynamics.Scenery.Zones;

namespace ZepLink.RiceNinja.Interfaces
{
    // Interface to make objects sleep outside of zones
    public interface ISceneryWakeable : IWakeable
    {
        Zone Zone { get; }
    }
}