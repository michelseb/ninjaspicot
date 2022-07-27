using ZepLink.RiceNinja.ServiceLocator;

namespace ZepLink.RiceNinja.Manageables
{
    public interface IManageable
    {
        /// <summary>
        /// Manageable id
        /// </summary>
        int Id { get; }

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