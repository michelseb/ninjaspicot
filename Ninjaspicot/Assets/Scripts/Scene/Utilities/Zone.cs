using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour, IWakeable
{
    [SerializeField] protected GameObject _centerObject;
    private Vector3? _center;
    public Vector3? Center
    {
        get
        {
            if (_centerObject == null || !_centerObject.activeInHierarchy)
                return null;

            if (_center == null) _center = _centerObject.transform.position;
            return _center;
        }
    }

    protected List<ISceneryWakeable> _wakeables;
    // Hack for door that is in the next zone
    [SerializeField] private List<GameObject> _additionalResettables;
    protected List<IResettable> _resettables;
    protected ZoneManager _zoneManager;
    protected Animator _animator;
    protected CheckPoint _checkpoint;
    protected SpawnManager _spawnManager;
    public ZoneManager ZoneManager { get { if (Utils.IsNull(_zoneManager)) _zoneManager = ZoneManager.Instance; return _zoneManager; } }

    private long _id;
    public long Id { get { if (_id == 0) _id = GetInstanceID(); return _id; } }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _spawnManager = SpawnManager.Instance;
    }

    protected virtual void Start()
    {
        _wakeables = GetComponentsInChildren<ISceneryWakeable>().ToList();
        _resettables = GetComponentsInChildren<IResettable>().ToList();
        ZoneManager.AddZone(this);
        _checkpoint = GetComponentInChildren<CheckPoint>();
    }

    public virtual void Open()
    {
        _animator.SetTrigger("Open");
        Wake();
    }

    public virtual void Close()
    {
        _animator.SetTrigger("Close");
        Sleep();
    }

    public void Wake()
    {
        for (int i = 0; i < _wakeables.Count; i++)
        {
            var item = _wakeables[i];

            if (Utils.IsNull(item))
            {
                _wakeables.RemoveAt(i);
                continue;
            }

            item.Wake();
            SetSpawn();
        }
    }

    public void Sleep()
    {
        for (int i = 0; i < _wakeables.Count; i++)
        {
            var item = _wakeables[i];

            if (Utils.IsNull(item))
            {
                _wakeables.RemoveAt(i);
                continue;
            }

            item.Sleep();
        }
    }

    protected virtual void SetSpawn()
    {
        _spawnManager.SetSpawn(_checkpoint);
    }

    public void ResetItems()
    {
        _resettables.ForEach(r => r.DoReset());
        _additionalResettables.ForEach(r => { if (r.TryGetComponent(out IResettable resettable)) resettable.DoReset(); });
        var poolables = FindObjectsOfType<MonoBehaviour>().OfType<IPoolable>().ToList();
        poolables.ForEach(p => p.Sleep());
    }
}
