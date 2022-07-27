using ZepLink.RiceNinja.Dynamics.Characters.Components.Hearing;

namespace ZepLink.RiceNinja.Dynamics.Interfaces
{
    public interface IListener : IDynamic
    {
        float Range { get; }
        void Hear(HearingArea hearingArea);
    }
}