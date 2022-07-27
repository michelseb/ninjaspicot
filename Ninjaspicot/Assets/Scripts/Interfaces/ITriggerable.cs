using System.Collections;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives;

namespace ZepLink.RiceNinja.Interfaces
{
    public interface ITriggerable
    {
        bool Triggered { get; }
        int LastTrigger { get; }
        void StartTrigger(EventTrigger trigger);
        IEnumerator Trigger(EventTrigger trigger);
        bool IsTriggeredBy(int triggerId);
    }
}