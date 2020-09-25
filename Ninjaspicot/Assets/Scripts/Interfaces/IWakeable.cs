// Interface to make objects sleep outside of zones
public interface IWakeable
{
    void Sleep();
    void Wake();
    Zone Zone { get; }
}
