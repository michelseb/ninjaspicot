using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Zone : MonoBehaviour
{
    private List<IWakeable> _wakeables;
    private ZoneManager _zoneManager;
    private Animator _animator;
    private CheckPoint _checkpoint;
    private SpawnManager _spawnManager;
    public ZoneManager ZoneManager { get { if (Utils.IsNull(_zoneManager)) _zoneManager = ZoneManager.Instance; return _zoneManager; } }

    private long _id;
    public long Id { get { if (_id == 0) _id = GetInstanceID(); return _id; } }

    public bool Exited { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _wakeables = GetComponentsInChildren<IWakeable>().ToList();
        Close();
        ZoneManager.AddZone(this);
        Exited = true;
        _spawnManager = SpawnManager.Instance;
        _checkpoint = GetComponentInChildren<CheckPoint>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        Exited = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
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
                _spawnManager.SetSpawn(_checkpoint);
            }
            else
            {
                _wakeables[i].Sleep();
            }
        }
    }
}
