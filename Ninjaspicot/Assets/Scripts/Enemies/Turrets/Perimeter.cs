using UnityEngine;

public class Perimeter : MonoBehaviour, IRaycastable
{
    private TurretBase _turret;
    public TurretBase Turret { get { if (_turret == null) _turret = GetComponentInParent<TurretBase>() ?? GetComponentInChildren<TurretBase>(); return _turret; } }
    public int Id => Turret.Id;

    private Canvas _canvas;
    private CameraBehaviour _cameraBehaviour;

    private void Start()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = _cameraBehaviour.MainCamera;
    }
}
