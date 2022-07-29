using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Manageables
{
    public abstract class IntManageable : Manageable<int>
    {
        private int _id;
        public override int Id { get { if (_id == default) _id = BaseUtils.GetRandomInt(); return _id; } }

        public virtual string Name => "Manageable";

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}
