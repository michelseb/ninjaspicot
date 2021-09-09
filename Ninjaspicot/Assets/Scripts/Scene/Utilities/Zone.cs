using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    protected List<IWakeable> _wakeables;
    protected ZoneManager _zoneManager;
    protected Animator _animator;
    protected CheckPoint _checkpoint;
    protected SpawnManager _spawnManager;
    public ZoneManager ZoneManager { get { if (Utils.IsNull(_zoneManager)) _zoneManager = ZoneManager.Instance; return _zoneManager; } }

    private long _id;
    public long Id { get { if (_id == 0) _id = GetInstanceID(); return _id; } }

    public bool Exited { get; protected set; }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        _wakeables = GetComponentsInChildren<IWakeable>().ToList();
        Close();
        ZoneManager.AddZone(this);
        Exited = true;
        _spawnManager = SpawnManager.Instance;
        _checkpoint = GetComponentInChildren<CheckPoint>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        Exited = true;
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        ZoneManager.SetZone(this);
        Exited = false;
    }

    public void Open()
    {
        SetItemsActivation(true);

        _animator.SetTrigger("Open");
    }

    public void Close()
    {
        SetItemsActivation(false);

        _animator.SetTrigger("Close");
    }

    private void SetItemsActivation(bool active)
    {
        for (int i = 0; i < _wakeables.Count; i++)
        {
            if (Utils.IsNull(_wakeables[i]))
            {
                _wakeables.RemoveAt(i);
                continue;
            }

            if (active)
            {
                _wakeables[i].Wake();
                SetSpawn();
            }
            else
            {
                _wakeables[i].Sleep();
            }
        }
    }

    protected virtual void SetSpawn()
    {
        _spawnManager.SetSpawn(_checkpoint);
    }
}
