using UnityEngine;

public class InstantBullet : MonoBehaviour, IPoolable
{
    public PoolableType PoolableType => PoolableType.None;

    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer { get { if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>(); return _lineRenderer; } }

    private ParticleSystem _particleSystem;
    public ParticleSystem ParticleSystem { get { if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>(); return _particleSystem; } }

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }


    public void Pool(Vector3 position, Quaternion rotation, float size)
    {
        _transform.position = new Vector3(position.x, position.y, -5);
        _transform.rotation = rotation;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
}
