using UnityEngine;

public class LaserThrower : Dynamic, ISceneryWakeable, IRaycastable, IResettable
{
    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }

    private PoolManager _poolManager;

    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    protected virtual void Awake()
    {
        _poolManager = PoolManager.Instance;
    }

    protected void OnTriggerEnter2D(Collider2D collision)

    {
        if (!collision.CompareTag("hero"))
            return;

        _poolManager.GetPoolable<ThrownLaser>(Transform.position, Quaternion.identity);
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
