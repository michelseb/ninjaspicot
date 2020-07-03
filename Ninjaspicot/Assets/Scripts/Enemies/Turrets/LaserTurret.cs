using System.Collections;

public class LaserTurret : TurretBase
{
    private LaserAim _laserAim;

    protected override void Awake()
    {
        base.Awake();
        _laserAim = _aim as LaserAim;
    }

    protected override void Aim()
    {
        base.Aim();

        if (_target != null && _aim.TargetAimedAt(_target, Id))
        {
            _laserAim.StartLaserize();
        }
        else
        {
            StartWait();
        }
    }

    protected override IEnumerator Wait()
    {
        yield return StartCoroutine(base.Wait());
        _laserAim.StopLaserize();
    }
}