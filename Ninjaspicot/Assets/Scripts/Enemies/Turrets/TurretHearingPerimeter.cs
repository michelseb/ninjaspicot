using UnityEngine;

public class TurretHearingPerimeter : HearingPerimeter
{
    private TurretBase _turretBase;
    public TurretBase TurretBase { get { if (_turretBase == null) _turretBase = GetComponentInParent<TurretBase>() ?? GetComponentInChildren<TurretBase>(); return _turretBase; } }
}
