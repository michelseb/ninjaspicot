using UnityEngine;

public class Perimeter : MonoBehaviour, IRaycastable
{
    private TurretBase _turret;
    public TurretBase Turret { get { if (_turret == null) _turret = GetComponentInParent<TurretBase>() ?? GetComponentInChildren<TurretBase>(); return _turret; } }
    public int Id => Turret.Id;
}
