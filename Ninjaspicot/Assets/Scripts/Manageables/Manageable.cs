using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Manageables
{
    public abstract class Manageable : IManageable
    {
        private int _id;
        public int Id { get { if (_id == default) _id = BaseUtils.GetRandomInt(); return _id; } }

        public virtual string Name => "Manageable";

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}
