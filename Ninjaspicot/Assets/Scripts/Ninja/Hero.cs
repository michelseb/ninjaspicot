using System.Collections;
using UnityEngine;

public class Hero : Ninja
{

    [SerializeField] private bool _getsCheckPoint;
    [SerializeField] private Transform _pos;
    public Transform Pos => _pos;


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

    public override IEnumerator Dying()
    {
        _timeManager.SetNormalTime();
        _timeManager.SetActive(false);

        yield return new WaitForSeconds(2);

        SetCapeActivation(false);
        _spawnManager.Respawn();
        SetCapeActivation(true);
        _cameraBehaviour.Center();
        _timeManager.SetActive(true);
        Movement.enabled = true;
    }

    public void SetCapeActivation(bool active)
    {
        _cape.GetComponent<SkinnedMeshRenderer>().enabled = active;
        _cape.enabled = active;
    }
}
