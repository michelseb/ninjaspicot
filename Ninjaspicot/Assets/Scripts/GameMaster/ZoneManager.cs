using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public List<Zone> Zones { get { if (_zones == null) _zones = new List<Zone>(); return _zones; } }
    public Zone CurrentZone { get; private set; }
    public long CurrentZoneId => _currentZoneId;

    private List<Zone> _zones;
    private long _currentZoneId;

    private CameraBehaviour _cameraBehaviour;

    private static ZoneManager _instance;
    public static ZoneManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ZoneManager>(); return _instance; } }

    private void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    public void SetZone(Zone zone)
    {
        if (zone.Id == _currentZoneId)
            return;

        if (CurrentZone)
        {
            CurrentZone.CloseForever();
        }
        else
        {
            for (int i = 0; i < Zones.Count; i++)
            {
                if (Zones[i] == zone)
                    continue;

                if (!Zones[i])
                {
                    Zones.RemoveAt(i);
                    continue;
                }
                Zones[i].Close();
            }
        }

        CurrentZone = zone;
        _currentZoneId = zone.Id;
        CurrentZone.Open();

        UpdateCurrentZoneCameraBehavior();
    }

    public void OpenExtraZone(Zone zone)
    {
        zone.Open();

        if (zone.Center.HasValue)
        {
            _cameraBehaviour.SetCenterMode(zone.Center.Value);
        }
        else
        {
            _cameraBehaviour.SetFollowMode();
        }
    }

    public void UpdateCurrentZoneCameraBehavior()
    {
        if (Utils.IsNull(CurrentZone))
            return;

        if (CurrentZone.Center.HasValue)
        {
            _cameraBehaviour.SetCenterMode(CurrentZone.Center.Value);
        }
        else
        {
            _cameraBehaviour.SetFollowMode();
        }
    }

    public void AddZone(Zone zone)
    {
        if (Zones.Contains(zone))
            return;

        Zones.Add(zone);
    }
}