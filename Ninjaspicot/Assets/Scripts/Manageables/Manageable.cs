﻿using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables
{
    public abstract class Manageable<T> : IManageable<T>
    {
        public abstract T Id { get; }

        public virtual string Name => "Manageable";

        public ServiceFinder ServiceFinder => ServiceFinder.Instance;
    }
}
