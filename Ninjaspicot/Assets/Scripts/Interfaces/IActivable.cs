using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Interfaces
{
    public interface IActivable
    {
        void Activate(IActivator activator = default);
        void Deactivate(IActivator activator = default);
    }
}