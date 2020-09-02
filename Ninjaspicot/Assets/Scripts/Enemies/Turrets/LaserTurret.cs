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

        if (Target != null && _aim.TargetAimedAt(Target, Id))
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