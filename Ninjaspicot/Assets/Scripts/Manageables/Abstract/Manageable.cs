using ZepLink.RiceNinja.Manageables.Interfaces;
using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables.Abstract
{
    public abstract class Manageable<T> : IManageable<T>
    {
        public abstract T Id { get; }

        public virtual string Name => "Manageable";

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}
