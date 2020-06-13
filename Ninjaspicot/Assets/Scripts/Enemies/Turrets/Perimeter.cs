using UnityEngine;

public class Perimeter : MonoBehaviour, IRaycastable
{
    private Turret _turret;
    public Turret Turret { get { if (_turret == null) _turret = transform.parent?.GetComponent<Turret>() ?? GetComponentInChildren<Turret>(); return _turret; } }
    public int Id => Turret.Id;
}
