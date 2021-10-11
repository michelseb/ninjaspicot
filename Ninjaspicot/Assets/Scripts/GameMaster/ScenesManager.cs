using System;
using System.Collections;
using System.Collections.Generic;
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
    public CustomColor FontColor;
    public CustomColor GlobalLightColor;
    public CustomColor FrontLightColor;
}


public class ScenesManager : MonoBehaviour
{
    [SerializeField] private SceneInfos[] _scenes;
    [SerializeField] private int _startScene;
    [SerializeField] private int _startCheckPoint;
    [SerializeField] private AudioClip[] _sceneAudios;

    public Coroutine SceneLoad { get; private set; }
    public SceneInfos CurrentScene { get; private set; }

    private List<ISceneryWakeable> _wakeables;

    private SpawnManager _spawnManager;
    private AudioSource _audioSource;
    private Coroutine _volumeDown;

    private static ScenesManager _instance;
    public static ScenesManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ScenesManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _spawnManager = SpawnManager.Instance;
        _audioSource = GetComponent<AudioSource>();

        if (_startScene < 2)
        {
            LoadLobby();
        }
        else
        {
            LoadSceneById(_startScene);
        }
    }

    private void Start()
    {
        _spawnManager.InitActiveSceneSpawns(_startCheckPoint);
    }

    public void LoadLobby()
    {
        var lobby = FindSceneByName("Lobby");
        SceneManager.LoadScene(lobby.Name);
        SwitchAudio(1);
        lobby.Loaded = true;

        // Wake lobby wakeables
        _wakeables = Utils.FindObjectsOfTypeInScene<ISceneryWakeable>("Lobby");
        _wakeables.ForEach(w => w.Wake());
    }

    public void LoadSceneById(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
        SwitchAudio(sceneId);
    }

    private void SwitchAudio(int sceneId)
    {
        if (_sceneAudios.Length >= sceneId)
        {
            _audioSource.Stop();
            _audioSource.clip = _sceneAudios[sceneId];
            _audioSource.volume = 1;
            _audioSource.Play();
        }
    }
    private IEnumerator VolumeDown()
    {
        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= Time.deltaTime;
            yield return null;
        }

        _volumeDown = null;
    }

    private IEnumerator LoadAdditionalZone(int portalId)
    {
        var scene = GetSceneByPortalId(portalId);

        if (scene == null || scene.Loaded)
            yield break;

        _volumeDown = StartCoroutine(VolumeDown());
        var operation = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (operation.progress < .9f || _volumeDown != null)
            yield return null;

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;

        EnableScene(scene);
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
            CurrentScene = sceneInfos;
            SwitchAudio(sceneToLoad.buildIndex);
            //Deactivate all scenery wakeables
            _wakeables = FindObjectsOfType<Zone>(true).SelectMany(zone => zone.GetComponentsInChildren<ISceneryWakeable>()).ToList();
            _wakeables.ForEach(w => w.Sleep());

            sceneInfos.Loaded = true;
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

    private SceneInfos GetSceneByPortalId(int portalId)
    {
        var id = int.Parse(portalId.ToString().Substring(0, 2));
        return FindSceneById(id);
    }

}
