namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IFocusable : IDynamic
    {
        bool IsSilent { get; }
        bool Taken { get; }
    }
}