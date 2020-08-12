using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public class SceneInfos
{
    public int Index;
    public string Name;
    public Sprite Img;
    public bool Loaded;
    public Color FontColor;
}


public class ScenesManager : MonoBehaviour
{
    [SerializeField] private SceneInfos[] _scenes;
    [SerializeField] private int _startScene;
    [SerializeField] private int _startCheckPoint;

    private SpawnManager _spawnManager;
    public Coroutine SceneLoad { get; private set; }

    private static ScenesManager _instance;
    public static ScenesManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ScenesManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _spawnManager = SpawnManager.Instance;
        if (_startScene < 2)
        {
            LoadLobby();
        }
        else
        {
            LoadSceneById(_startScene);
        }
        _spawnManager.InitActiveSceneSpawns(_startCheckPoint - 1);
    }

    public void LoadLobby()
    {
        var lobby = FindSceneByName("Lobby");
        SceneManager.LoadScene(lobby.Name);
        lobby.Loaded = true;
    }

    public void LoadSceneById(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    private IEnumerator LoadAdditionalZone(int portalId)
    {
        var id = int.Parse(portalId.ToString().Substring(0, 2));
        var scene = FindSceneById(id);

        if (scene == null || scene.Loaded)
            yield break;

        var operation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (operation.progress < .9f)
            yield return null;

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;

        EnableScene(scene);
        scene.Loaded = true;
        SceneLoad = null;
    }

    private void EnableScene(SceneInfos sceneInfos)
    {
        Scene sceneToLoad = SceneManager.GetSceneByName(sceneInfos.Name);
        if (sceneToLoad.IsValid())
        {
            SceneManager.MoveGameObjectToScene(Hero.Instance.gameObject, sceneToLoad);
            SceneManager.SetActiveScene(sceneToLoad);
            _spawnManager.InitActiveSceneSpawns();
        }
    }

    public void LaunchZoneLoad(int portalId)
    {
        if (SceneLoad != null)
            return;

        SceneLoad = StartCoroutine(LoadAdditionalZone(portalId));
    }

    public void UnloadZone(int zoneId)
    {
        var scene = FindSceneById(zoneId);

        if (scene == null || !scene.Loaded)
            return;

        SceneManager.UnloadSceneAsync(scene.Name);
        scene.Loaded = false;
    }


    private SceneInfos FindSceneById(int id)
    {
        return _scenes.FirstOrDefault(s => s.Index == id);
    }

    private SceneInfos FindSceneByName(string name)
    {
        return _scenes.FirstOrDefault(s => s.Name == name);
    }

    public void MoveObjectToCurrentScene(GameObject obj)
    {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
    }

}
