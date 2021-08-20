public class EnemyHearingPerimeter : HearingPerimeter
{
    private Enemy _enemy;
    public Enemy Enemy { get { if (_enemy == null) _enemy = GetComponentInParent<Enemy>() ?? GetComponentInChildren<Enemy>(); return _enemy; } }
}
