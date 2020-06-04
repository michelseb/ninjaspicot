using System.Collections;
using UnityEngine;

public class Hero : Ninja, IRaycastable
{
    [SerializeField] private bool _getsCheckPoint;

    private int _id;
    public int Id { get { if (_id == 0) _id = gameObject.GetInstanceID(); return _id; } }
    public bool Triggered { get; private set; }

    private int _lastTrigger;
    private Cloth _cape;
    private TimeManager _timeManager;
    private SpawnManager _spawnManager;

    private static Hero _instance;
    public static Hero Instance { get { if (_instance == null) _instance = FindObjectOfType<Hero>(); return _instance; } }


    protected override void Awake()
    {
        base.Awake();
        _timeManager = TimeManager.Instance;
        _spawnManager = SpawnManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _cape = GetComponentInChildren<Cloth>();
    }

    public override void Die(Transform killer)
    {
        if (Dead)
            return;

        base.Die(killer);
        _timeManager.StartSlowDownProgressive(.3f);
    }

    public override IEnumerator Dying()
    {
        yield return new WaitForSeconds(1);

        SetCapeActivation(false);
        _spawnManager.Respawn();
        SetMovementAndStickinessActivation(true);
        _cameraBehaviour.SetCenterMode(transform, 1f);
        SetCapeActivation(true);

        yield return new WaitForSeconds(1f);

        _cameraBehaviour.SetFollowMode(transform);
    }

    public void SetCapeActivation(bool active)
    {
        _cape.GetComponent<SkinnedMeshRenderer>().enabled = active;
        _cape.enabled = active;
    }

    public IEnumerator Trigger(EventTrigger trigger)
    {
        Triggered = true;
        _lastTrigger = trigger.Id;
        
        yield return new WaitForSeconds(3);
        
        Triggered = false;

        if (trigger.SingleTime)
        {
            Destroy(trigger.gameObject);
        }
    }

    public bool IsTriggeredBy(int id)
    {
        return _lastTrigger == id;
    }
}
