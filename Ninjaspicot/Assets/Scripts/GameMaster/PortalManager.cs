using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class DoorPair
{
    public int EntranceId;
    public int ExitId;
}


public class PortalManager : MonoBehaviour
{
    [SerializeField] private DoorPair[] _doorEntranceExitPairs;

    public bool Connecting { get; private set; }
    public Coroutine Connection { get; private set; }
    public List<Portal> Portals { get { if (_portals == null) _portals = new List<Portal>(); return _portals; } }


    private List<Portal> _portals;
    private ScenesManager _scenesManager;
    private CameraBehaviour _cameraBehaviour;
    private UICamera _uiCamera;
    private AudioSource _audioSource;
    private AudioManager _audioManager;
    private AudioClip _enterClip;
    private AudioClip _exitClip;


    public const int TRANSFER_SPEED = 3; //Seconds needed to go between 2 portals
    public const float EJECT_SPEED = 100; //How strongly transferred hero is ejected

    private static PortalManager _instance;
    public static PortalManager Instance { get { if (_instance == null) _instance = FindObjectOfType<PortalManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _scenesManager = ScenesManager.Instance;
        _cameraBehaviour = CameraBehaviour.Instance;
        _uiCamera = UICamera.Instance;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
    }

    private void Start()
    {
        _enterClip = _audioManager.FindByName("EnterPortal");
        _exitClip = _audioManager.FindByName("ExitPortal");
    }

    private int? GetExitIndexByEntranceId(int entranceId)
    {
        return _doorEntranceExitPairs.FirstOrDefault(d => d.EntranceId == entranceId)?.ExitId;
    }

    public Portal GetPortalById(int portalId)
    {
        return Portals.FirstOrDefault(t => t.Id == portalId);
    }

    private IEnumerator CreateConnection(Portal entrance, int exitId)
    {
        Connecting = true;

        _scenesManager.LaunchZoneLoad(exitId);

        while (_scenesManager.SceneLoad != null)
            yield return null;

        var exit = GetPortalById(exitId);

        if (exit == null)
        {
            TerminateConnection();
            yield break;
        }

        entrance.Entrance = true;
        exit.Exit = true;

        entrance.SetOtherPortal(exit);
        exit.SetOtherPortal(entrance);
        Connection = null;
    }

    public void LaunchConnection(Portal entrance)
    {
        if (Connection != null)
            return;

        var otherId = GetExitIndexByEntranceId(entrance.Id);

        if (otherId == null)
            return;

        _scenesManager.InitColorChange(otherId.Value);
        Connection = StartCoroutine(CreateConnection(entrance, otherId.Value));
    }

    public void ClosePreviousZone(int entranceId)
    {
        var zoneId = int.Parse(entranceId.ToString().Substring(0, 2));

        RemoveZonePortalsFromList(zoneId);
        _scenesManager.UnloadZone(zoneId);
    }

    public void TerminateConnection()
    {
        Connecting = false;
    }

    public void AddPortal(Portal portal)
    {
        if (Portals.Contains(portal))
            return;

        Portals.Add(portal);
    }

    private void RemoveZonePortalsFromList(int zoneId)
    {
        Portals.Where(p => p.Id.ToString().Substring(0, 2) == zoneId.ToString()).ToList().ForEach(x => Portals.Remove(x));
    }

    public IEnumerator Teleport(Portal entrance, Portal exit)
    {
        _audioSource.PlayOneShot(_exitClip, .3f);
        var rb = Hero.Instance.Stickiness.Rigidbody;
        rb.position = exit.transform.position - exit.transform.right * 4;
        _cameraBehaviour.Teleport(Hero.Instance.Stickiness.Rigidbody.position);
        _uiCamera.CameraAppear();
        entrance.Reinit();
        ClosePreviousZone(entrance.Id);

        yield return new WaitForSecondsRealtime(2);

        Hero.Instance.StartAppear();
        rb.isKinematic = false;
        rb.velocity = exit.transform.right * EJECT_SPEED;
        Hero.Instance.SetCapeActivation(true);
        exit.Reinit();
        TerminateConnection();
    }

    public void StartPortalSound()
    {
        _audioSource.PlayOneShot(_enterClip, .3f);
    }
}
