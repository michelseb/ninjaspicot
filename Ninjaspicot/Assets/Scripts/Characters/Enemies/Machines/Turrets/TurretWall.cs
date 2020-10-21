using UnityEngine;

public class TurretWall : MonoBehaviour, IRaycastable
{
    protected TurretBase _turret;
    public TurretBase Turret { get { if (_turret == null) _turret = GetComponentInChildren<TurretBase>() ?? GetComponentInParent<TurretBase>(); return _turret; } }
    public int Id => Turret.Id;

    protected Canvas _canvas;
    protected CameraBehaviour _cameraBehaviour;

    private void Start()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
        _canvas = GetComponent<Canvas>();
        if (_canvas != null)
        {
            _canvas.worldCamera = _cameraBehaviour.MainCamera;
        }
    }
}
