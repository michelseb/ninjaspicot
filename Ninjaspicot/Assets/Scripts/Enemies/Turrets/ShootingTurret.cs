using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShootingTurret : TurretBase
{
    [SerializeField] private bool _autoShoot;
    [SerializeField] private float _strength;
    [SerializeField] private float _loadTime;

    public bool AutoShoot => _autoShoot;

    private float _loadProgress;

    private PoolManager _poolManager;



    protected override void Update()
    {
        _image.color = Active ? ColorUtils.Red : ColorUtils.White;

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


        switch (TurretMode)
        {
            case Mode.Aim:

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, _target.transform.position - transform.position), .05f);

                if (_target != null && _aim.TargetAimedAt(_target, Id))
                {
                    if (Loaded && _aim.TargetCentered(transform, _target.tag, Id))
                    {
                        Shoot();
                    }
                }
                else
                {
                    StartWait();
                }
                break;


            case Mode.Scan:

                if (_autoShoot)
                {
                    if (Loaded)
                    {
                        Shoot();
                    }
                }

                var dir = _clockWise ? 1 : -1;

                transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime * dir);

                if (dir * (transform.rotation.eulerAngles.z - _initRotation) > _viewAngle)
                {
                    _clockWise = !_clockWise;
                }

                break;


            case Mode.Wait:

                if (_target != null && _aim.TargetAimedAt(_target, Id))
                {
                    StartAim(_target);
                }
                break;
        }

    }

    private void Shoot()
    {
        var bullet = _poolManager.GetPoolable<Bullet>(transform.position, transform.rotation, PoolableType.Bullet);
        bullet.Speed = _strength;
        Loaded = false;
    }
}