using System.Collections;
using UnityEngine;

public class RobotLaser : Dynamic, IActivable
{
    [SerializeField] protected ParticleSystem _dust;
    [SerializeField] protected ParticleSystem _charge;
    protected LineRenderer _laser;
    protected Enemy _enemy;
    private AudioManager _audioManager;
    private Audio _electrocutionSound;
    protected int _pointsAmount;
    protected bool _active;

    protected virtual void Awake()
    {
        _laser = GetComponent<LineRenderer>();
        _enemy = GetComponentInParent<Enemy>();
        _pointsAmount = _laser.positionCount;
        _audioManager = AudioManager.Instance;
    }

    protected virtual void Start()
    {
        _electrocutionSound = _audioManager.FindAudioByName("Electrocution");
    }

    protected void Update()
    {
        Cast();
    }

    private void Cast()
    {
        if (!_active)
            return;
        var cast = Utils.RayCast(Transform.position, Transform.right, ignore: _enemy.Id, includeTriggers: false);

        if (!cast)
            return;

        for (int i = 1; i < _pointsAmount; i++)
        {
            var pos = transform.InverseTransformPoint(cast.point) / (_pointsAmount - i);
            _laser.SetPosition(i, new Vector2(pos.x, pos.y) + new Vector2(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)));
        }
        _dust.transform.position = cast.point;

        if (cast.collider.CompareTag("hero"))
        {
            Hero.Instance.Die(sound: _electrocutionSound, volume: .4f);
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoActivate());
    }

    private IEnumerator DoActivate()
    {
        _charge.Play();
        yield return new WaitForSeconds(1.5f);
        _active = true;
        _laser.enabled = true;
        _dust.Play();
    }

    public void Deactivate()
    {
        _laser.enabled = false;
        _active = false;
        _charge.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _dust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameObject.SetActive(false);
    }
}
