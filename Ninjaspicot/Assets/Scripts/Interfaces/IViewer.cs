public interface IViewer : IRaycastable
{
    Aim AimField { get; }
    IKillable Target { get; set; }
    void StartAim(IKillable target);
    void Aim(IKillable target);
    void LookFor();
}
