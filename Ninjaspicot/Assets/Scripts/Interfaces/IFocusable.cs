public interface IFocusable
{
    bool IsSilent { get; }
    bool Taken { get; }
    bool FocusedByNormalJump { get; }
}
