using UnityEngine;

public class CameraCatcher : MonoBehaviour, IActivable
{
    [SerializeField] protected int _zoomAmount;
    protected CameraBehaviour _cameraBehaviour;
    public virtual Transform ZoomCenter => transform;

    private bool _activated;

    protected virtual void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            Activate();
        }
    }
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") && !_activated)
        {
            Activate();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            Deactivate();
        }
    }

    public virtual void Activate()
    {
        if (_cameraBehaviour.CameraMode == CameraMode.Follow)
        {
            _cameraBehaviour.Zoom(ZoomType.Progressive, _zoomAmount);
            _cameraBehaviour.SetCenterMode(ZoomCenter, .5f);
            _activated = true;
        }
    }

    public virtual void Deactivate()
    {
        _cameraBehaviour.Zoom(ZoomType.Init);
        _cameraBehaviour.SetFollowMode(Hero.Instance.transform);
        _activated = false;
    }
}
