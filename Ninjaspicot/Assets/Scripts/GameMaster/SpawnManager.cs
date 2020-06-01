using System.Linq;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Hero _spawnEntity;

    private Vector3 _spawnPosition;
    private CheckPoint[] _checkPoints;
    
    private static SpawnManager _instance;
    public static SpawnManager Instance { get { if (_instance == null) _instance = FindObjectOfType<SpawnManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitSpawns();
        if (Hero.Instance == null)
        {
            Instantiate(_spawnEntity, _spawnPosition, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (Hero.Instance == null)
        {
            Instantiate(_spawnEntity, _spawnPosition, Quaternion.identity);
        }
    }

    public void Respawn()
    {
        if (!_checkPoints.Any(c => c.Attained))
        {
            Hero.Instance.transform.position = _spawnPosition;
            return;
        }

        var pos = _checkPoints.Last(c => c.Attained).transform.position;
        Hero.Instance.transform.position = pos;
    }

    public void InitSpawns()
    {
        _checkPoints = FindObjectsOfType<CheckPoint>();
        var spawn = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        _spawnPosition = new Vector3(spawn.x, spawn.y, -5);
    }
}
