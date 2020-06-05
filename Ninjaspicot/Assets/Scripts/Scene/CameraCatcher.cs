using UnityEngine;

public class CameraCatcher : MonoBehaviour
{
    [SerializeField] private int _zoomAmount;
    private bool _activated;
    private CameraBehaviour _cameraBehaviour;
    private float _previousZoom;
    private void Awake()
    {
        _cameraBehaviour = CameraBehaviour.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _previousZoom = _cameraBehaviour.Camera.orthographicSize;
        if (collision.CompareTag("hero"))
        {
            if (_cameraBehaviour.CameraMode == CameraMode.Follow)
            {
                _cameraBehaviour.Zoom(ZoomType.Progressive, _zoomAmount);
                _cameraBehaviour.SetCenterMode(transform, .5f);
                _activated = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") && _activated)
        {
            _cameraBehaviour.Zoom(ZoomType.Progressive, -_zoomAmount);
            _cameraBehaviour.SetFollowMode(Hero.Instance.transform);
        }
        _activated = false;
    }
}
