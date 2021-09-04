public interface IListener
{
    float Range { get; }
    void Hear(HearingArea hearingArea);
}
