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


    private List<Portal> _portals;
    private ScenesManager _scenesManager;
    public List<Portal> Portals { get { if (_portals == null) _portals = new List<Portal>(); return _portals; } }

    private static PortalManager _instance;
    public static PortalManager Instance { get { if (_instance == null) _instance = FindObjectOfType<PortalManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _scenesManager = ScenesManager.Instance;
    }

    private int? GetExitIndexByEntranceId(int entranceId)
    {
        return _doorEntranceExitPairs.FirstOrDefault(d => d.EntranceId == entranceId)?.ExitId;
    }

    public Portal GetPortalById(int portalId)
    {
        return Portals.FirstOrDefault(t => t.Id == portalId);
    }

    private IEnumerator CreateConnection(Portal entrance)
    {
        Connecting = true;
        var otherId = GetExitIndexByEntranceId(entrance.Id);

        if (otherId == null)
        {
            TerminateConnection();
            yield break;
        }

        _scenesManager.LaunchZoneLoad((int)otherId);

        while (_scenesManager.SceneLoad != null)
            yield return null;

        var exit = GetPortalById((int)otherId);

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

        Connection = StartCoroutine(CreateConnection(entrance));
    }

    public void ClosePreviousZone(int entranceId)
    {
        var zoneId = int.Parse(entranceId.ToString().Substring(0, 2));

        RemoveZonePortalsFromList(zoneId);
        _scenesManager.UnloadZone(zoneId);
        TerminateConnection();
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
}
