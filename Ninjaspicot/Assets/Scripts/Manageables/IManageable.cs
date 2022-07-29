using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables
{
    public interface IManageable<T>
    {
        /// <summary>
        /// Manageable id
        /// </summary>
        T Id { get; }

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