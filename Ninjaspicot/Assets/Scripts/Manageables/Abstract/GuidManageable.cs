using System;

namespace ZepLink.RiceNinja.Manageables.Abstract
{
    public abstract class GuidManageable : Manageable<Guid>
    {
        private Guid _id;
        public override Guid Id { get { if (_id == default) _id = Guid.NewGuid(); return _id; } }
    }
}
