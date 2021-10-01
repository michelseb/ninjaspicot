using System.Collections;

public class LaserTurret : TurretBase
{
    private LaserAim _laserAim;

    protected override void Awake()
    {
        base.Awake();
        _laserAim = _aim as LaserAim;
    }

    public override void Aim(IKillable target)
    {
        base.Aim(target);

        if (TargetEntity != null && _aim.TargetAimedAt(TargetEntity, Id))
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

    public override void DoReset()
    {
        Dead = false;
        Wake();
    }
}