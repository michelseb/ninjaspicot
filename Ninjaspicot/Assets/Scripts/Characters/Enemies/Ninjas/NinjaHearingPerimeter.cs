public class NinjaHearingPerimeter : HearingPerimeter
{
    private EnemyNinja _ninja;
    public EnemyNinja Ninja { get { if (_ninja == null) _ninja = GetComponentInParent<EnemyNinja>() ?? GetComponentInChildren<EnemyNinja>(); return _ninja; } }
}
