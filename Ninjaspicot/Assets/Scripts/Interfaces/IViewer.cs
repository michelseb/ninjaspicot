public interface IViewer : IRaycastable
{
    Aim AimField { get; }
    IKillable TargetEntity { get; set; }
    void StartAim(IKillable target);
    void Aim(IKillable target);
    void LookFor();
}
