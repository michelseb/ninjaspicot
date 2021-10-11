using UnityEngine;

public class EnemyLaser : MonoBehaviour, IActivable
{
    protected LineRenderer _laser;
    protected Transform _transform;
    protected ParticleSystem _dust;
    protected Enemy _enemy;
    private AudioManager _audioManager;
    private Audio _electrocutionSound;
    protected int _pointsAmount;
    protected bool _active;
    public bool Active => _active;

    protected virtual void Awake()
    {
        _laser = GetComponent<LineRenderer>();
        _transform = transform;
        _dust = GetComponentInChildren<ParticleSystem>();
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
        var cast = Utils.RayCast(_transform.position, _transform.right, ignore: _enemy.Id, includeTriggers: false);
        
        if (cast)
        {
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

    }

    public void SetActive(bool active)
    {
        _active = active;
        if (_active)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        if (_active)
        {
            gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
