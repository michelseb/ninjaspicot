using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables.Interfaces
{
    public interface IManageable<T> : IManageable
    {
        /// <summary>
        /// Manageable id
        /// </summary>
        T Id { get; }
    }

    public interface IManageable
    {
        /// <summary>
        /// Manageable name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Service locator
        /// </summary>
        ServiceFinder ServiceFinder { get; }
    }
}