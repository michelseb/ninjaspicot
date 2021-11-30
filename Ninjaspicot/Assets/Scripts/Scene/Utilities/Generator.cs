﻿using UnityEngine;

public class Generator : Dynamic, ISceneryWakeable, IResettable, IBreakable
{
    [SerializeField] protected Laser[] _lasers;
    
    private Zone _zone;
    public Zone Zone { get { if (Utils.IsNull(_zone)) _zone = GetComponentInParent<Zone>(); return _zone; } }

    public bool IsSilent => false;
    public bool Taken => false;

    public bool Broken { get; set; }

    public bool Charge => true;

    private Lamp _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Lamp>();
    }

    public void DoReset()
    {
        _light.Light.enabled = true;

        Wake();

        foreach (var laser in _lasers)
        {
            laser.DoReset();
        }
    }

    public void Sleep()
    {
        _light.Sleep();
    }

    public void Wake()
    {
        _light.Wake();
    }

    public bool Break(Transform killer = null, Audio sound = null, float volume = 1)
    {
        if (Broken)
            return false;

        Broken = true;
        _light.Light.enabled = false;

        foreach (var laser in _lasers)
        {
            laser.Deactivate();
        }

        return true;
    }

    public void GoTo()
    {
        Break();
    }
}
