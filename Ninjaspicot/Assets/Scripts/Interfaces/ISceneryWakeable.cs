// Interface to make objects sleep outside of zones
public interface ISceneryWakeable : IWakeable
{
    Zone Zone { get; }
}
