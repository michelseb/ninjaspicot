using UnityEngine;

public class LaserThrower : MonoBehaviour, ISceneryWakeable, IRaycastable, IResettable
{
    [SerializeField] private float _spawnInterval;

    private float _timeLeftBeforeSpawn;
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private PoolManager _poolManager;

    private Transform _transform;
    public Transform Transform { get { if (Utils.IsNull(_transform)) _transform = transform; return _transform; } }

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _poolManager = PoolManager.Instance;
    }

    protected virtual void Update()
    {
        _timeLeftBeforeSpawn -= Time.deltaTime;
        if (_timeLeftBeforeSpawn <= 0)
        {
            _poolManager.GetPoolable<ThrownLaser>(Transform.position, Quaternion.identity);
            _timeLeftBeforeSpawn = _spawnInterval;
        }
    }

    public void Sleep()
    {
        gameObject.SetActive(false);
    }

    public void Wake()
    {
        gameObject.SetActive(true);
    }

    public void DoReset()
    {
    }
}
