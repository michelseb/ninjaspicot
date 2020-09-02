using UnityEngine;

public class ShootingTurret : TurretBase
{
    [SerializeField] private bool _autoShoot;
    [SerializeField] private float _strength;
    [SerializeField] private float _loadTime;

    public bool AutoShoot => _autoShoot;

    private float _loadProgress;
    
    private PoolManager _poolManager;

    protected override void Start()
    {
        base.Start();
        _poolManager = PoolManager.Instance;
    }


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

    protected override void Aim()
    {
        base.Aim();

        if (Target != null && _aim.TargetAimedAt(Target, Id))
        {
            if (Loaded && _aim.TargetCentered(_transform, Target.Transform.tag, Id))
            {
                Shoot();
            }
        }
        else if (Target == null || !_aim.TargetInRange)
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
        var bullet = _poolManager.GetPoolable<Bullet>(_transform.position, _transform.rotation, 1, PoolableType.Bullet);
        bullet.Speed = _strength;
        Loaded = false;
    }
}