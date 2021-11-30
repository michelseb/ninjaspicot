using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisibilityManager : MonoBehaviour
{
    private static VisibilityManager _instance;
    public static VisibilityManager Instance { get { if (_instance == null) _instance = FindObjectOfType<VisibilityManager>(); return _instance; } }

    private Hero _hero;
    public Hero Hero { get { if (_hero == null) _hero = Hero.Instance; return _hero; } }

    private ZoneManager _zoneManager;
    private long _currentZoneId;

    private List<IRevealer> _revealers;

    //private int _revelatorsAmount;

    public bool Visible { get; private set; } //=> _revelatorsAmount > 0;

    private void Awake()
    {
        _zoneManager = ZoneManager.Instance;
    }

    private void Update()
    {
        // Update revealers when changing zone
        if (_currentZoneId != _zoneManager.CurrentZoneId)
        {
            _revealers = _zoneManager.CurrentZone.GetComponentsInChildren<IRevealer>().ToList();
            _currentZoneId = _zoneManager.CurrentZoneId;
        }

        Visible = IsVisible();
        
    }


    private bool IsVisible()
    {
        if (Utils.IsNull(_revealers) || _revealers.Count == 0)
            return false;

        foreach (var revealer in _revealers)
        {
            if (revealer.Broken || Vector2.Distance(Hero.Transform.position, revealer.Transform.position) > revealer.ActivationDistance)
                continue;

            var hit = Utils.LineCast(revealer.Transform.position, Hero.Transform.position, Hero.Id);

            if (!hit)
            {
                return true;
            }
            else
            {
                Debug.Log(hit.transform.name);
            }
        }

        return false;
    }

    //public void AddRevelator()
    //{
    //    _revelatorsAmount++;
    //}

    //public void RemoveRevelator()
    //{
    //    _revelatorsAmount--;

    //    if (_revelatorsAmount < 0)
    //        _revelatorsAmount = 0;
    //}

}
