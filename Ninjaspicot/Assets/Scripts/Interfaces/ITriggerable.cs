using System.Collections;

public interface ITriggerable
{
    bool Triggered { get; }
    int LastTrigger { get; }
    void StartTrigger(EventTrigger trigger);
    IEnumerator Trigger(EventTrigger trigger);
    bool IsTriggeredBy(int triggerId);
}
