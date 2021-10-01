using UnityEngine;

public class ShootingTurret : TurretBase
{
    [SerializeField] private bool _autoShoot;
    [SerializeField] private float _strength;
    [SerializeField] private float _loadTime;

    public bool AutoShoot => _autoShoot;

    private float _loadProgress;

    protected override void Update()
    {
        base.Update();

        if (!Active)
            return;

        //Loading weapon
        if (!Loaded)
        {
            if (_loadProgress >= _loadTime)
            {
                Loaded = true;
                _loadProgress = 0;
            }
            else
            {
                _loadProgress += Time.deltaTime;
            }
        }
    }

    public override void Aim(IKillable target)
    {
        base.Aim(target);

        if (TargetEntity != null && _aim.TargetAimedAt(TargetEntity, Id))
        {
            if (Loaded && _aim.TargetCentered(Transform, TargetEntity.Transform.tag, Id))
            {
                Shoot();
            }
        }
        else if (TargetEntity == null || !_aim.TargetInRange)
        {
            StartWait();
        }
    }

    protected override void Scan()
    {
        if (_autoShoot)
        {
            if (Loaded)
            {
                Shoot();
            }
        }

        base.Scan();
    }

    private void Shoot()
    {
        var bullet = _poolManager.GetPoolable<Bullet>(Transform.position, Transform.rotation, 1, PoolableType.Bullet);
        bullet.Speed = _strength;
        Loaded = false;
    }

    public override void DoReset()
    {
        Dead = false;
        Wake();
    }
}