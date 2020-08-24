using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{

    private static ZoneManager _instance;
    public static ZoneManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ZoneManager>(); return _instance; } }

    private List<Zone> _zones;
    public List<Zone> Zones { get { if (_zones == null) _zones = new List<Zone>(); return _zones; } }
    public Zone CurrentZone { get; private set; }

    public void SetZone(Zone zone)
    {
        if (zone == CurrentZone)
            return;

        if (CurrentZone)
        {
            CurrentZone.StartClose();
        }
        else
        {
            for (int i = 0; i < Zones.Count; i++)
            {
                if (!Zones[i])
                {
                    Zones.RemoveAt(i);
                    continue;
                }
                Zones[i].StartClose();
            }
        }

        CurrentZone = zone;
        CurrentZone.StartOpen();
    }

    public void AddZone(Zone zone)
    {
        if (Zones.Contains(zone))
            return;

        Zones.Add(zone);
    }

}