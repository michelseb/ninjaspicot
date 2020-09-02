using System.Collections;
using UnityEngine;

public class InstantShootingTurret : TurretBase
{
    [SerializeField] private float _loadTime;

    private float _loadProgress;
    private PoolManager _poolManager;
    private AudioSource _audioSource;
    private AudioManager _audioManager;

    private const float SHOOTING_TIME = .08f;

    protected override void Awake()
    {
        base.Awake();
        _poolManager = PoolManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
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

    protected override void LookFor()
    {
        base.LookFor();
        var dir = Vector3.Dot(_transform.up, (Target.Transform.position - _transform.position).normalized);
        if (dir > .98f)
        {
            StartWait();
        }
    }


    protected override void Aim()
    {
        base.Aim();

        if (Target != null && _aim.TargetAimedAt(Target, Id))
        {
            if (Loaded)
            {
                StartShooting();
            }
        }
        else if (Target == null || !_aim.TargetInRange)
        {
            StartWait();
        }
    }

    private void StartShooting()
    {
        Loaded = false;
        StartCoroutine(Shoot(SHOOTING_TIME));
        _audioManager.PlaySound(_audioSource, "Gun");

        if (_aim.TargetCentered(_transform, Target.Transform.tag, Id))
        {
            Target.Die(_transform);
        }
    }

    private IEnumerator Shoot(float time)
    {
        var startPos = _transform.position + _transform.up * AimField.Offset.magnitude;
        var bullet = _poolManager.GetPoolable<InstantBullet>(startPos, _transform.rotation);
        var ray = Utils.RayCast(startPos, _transform.up, AimField.Size * 2, ignore: Id, includeTriggers: false);
        var line = bullet.LineRenderer;
        line.positionCount = 2;
        line.SetPosition(0, _transform.position);
        line.SetPosition(1, ray.point);

        yield return new WaitForSeconds(time);

        line.positionCount = 0;

        bullet.Deactivate();
    }

}