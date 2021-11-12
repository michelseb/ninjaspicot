using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Hero _spawnEntity;

    private Vector3 _spawnPosition;
    private CheckPoint[] _checkPoints;
    private TimeManager _timeManager;
    private CameraBehaviour _cameraBehaviour;

    private static SpawnManager _instance;
    public static SpawnManager Instance { get { if (_instance == null) _instance = FindObjectOfType<SpawnManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _timeManager = TimeManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    private void Start()
    {
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
        var hero = Hero.Instance;

        if (hero == null)
            return;

        hero.Stickiness.StopWalking(false);
        hero.Stickiness.Rigidbody.velocity = Vector2.zero;
        hero.Stickiness.Rigidbody.rotation = 0;
        hero.Stickiness.Rigidbody.isKinematic = false;
        hero.Stickiness.CurrentAttachment = null;
        hero.Dead = false;
        hero.Renderer.color = Color.white;
        _timeManager.SetNormalTime();
        _cameraBehaviour.Zoom(ZoomType.Init);

        hero.transform.position = _spawnPosition;
    }

    public void InitActiveSceneSpawns(int checkPoint = 0)
    {
        var scene = SceneManager.GetActiveScene();
        var sceneObjects = scene.GetRootGameObjects();

        _checkPoints = FindObjectsOfType<CheckPoint>()
            .OrderBy(c => c.Order)
            .ToArray(); //TODO : make checkpoint specific to scene => Add property in class to link to scene


        var spawn = _checkPoints.FirstOrDefault(x => x.Order == checkPoint)?.transform.position ?? _checkPoints[0].transform.position;

        _spawnPosition = new Vector3(spawn.x, spawn.y, -5);
    }

    public void SetSpawn(CheckPoint checkPoint)
    {
        if (checkPoint.Attained)
            return;

        _spawnPosition = new Vector3(checkPoint.transform.position.x, checkPoint.transform.position.y, -5);
        checkPoint.Attain();
    }
}
