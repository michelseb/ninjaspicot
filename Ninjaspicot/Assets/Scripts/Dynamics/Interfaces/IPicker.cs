namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IPicker : IDynamic
    {
        /// <summary>
        /// Is picker able to pick
        /// </summary>
        bool CanTake { get; }
    }
}