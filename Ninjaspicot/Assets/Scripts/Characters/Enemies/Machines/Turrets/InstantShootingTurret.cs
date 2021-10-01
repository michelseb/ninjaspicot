using System.Collections;
using UnityEngine;

public class InstantShootingTurret : TurretBase
{
    [SerializeField] private float _loadTime;

    private float _loadProgress;

    private const float SHOOTING_TIME = .08f;

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

    public override void LookFor()
    {
        base.LookFor();
        var dir = Vector3.Dot(_turretHead.up, (TargetEntity.Transform.position - _turretHead.position).normalized);
        if (dir > .98f)
        {
            StartWait();
        }
    }


    public override void Aim(IKillable target)
    {
        base.Aim(target);

        if (!_aim.TargetInRange)
        {
            StartWait();
        }
        else
        {
            if (TargetEntity != null && _aim.TargetAimedAt(TargetEntity, Id))
            {
                if (Loaded)
                {
                    StartShooting();
                }
            }
            else
            {
                StartWait();
            }
        }
    }

    private void StartShooting()
    {
        Loaded = false;
        StartCoroutine(Shoot(SHOOTING_TIME));
        _audioManager.PlaySound(_audioSource, "Gun");

        if (_aim.TargetCentered(_shootingPosition, TargetEntity.Transform.tag, Id))
        {
            TargetEntity.Die(_turretHead);
        }
    }

    private IEnumerator Shoot(float time)
    {
        var bullet = _poolManager.GetPoolable<InstantBullet>(_turretHead.position, _turretHead.rotation);
        var ray = Utils.RayCast(_turretHead.position, _turretHead.up, AimField.Size * 2, ignore: Id, includeTriggers: false);
        var line = bullet.LineRenderer;
        line.positionCount = 2;
        line.SetPosition(0, _turretHead.position);
        line.SetPosition(1, ray.point);

        yield return new WaitForSeconds(time);

        line.positionCount = 0;

        bullet.Deactivate();
    }

    public override void DoReset()
    {
        Dead = false;
        Wake();
    }

}