using UnityEngine;

public class Perimeter : MonoBehaviour, IRaycastable
{
    private ShootingTurret _turret;
    public ShootingTurret Turret { get { if (_turret == null) _turret = GetComponentInParent<ShootingTurret>() ?? GetComponentInChildren<ShootingTurret>(); return _turret; } }
    public int Id => Turret.Id;
}
