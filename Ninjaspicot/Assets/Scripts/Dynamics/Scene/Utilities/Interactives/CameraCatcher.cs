using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class CameraCatcher : Dynamic, IActivable
    {
        [SerializeField] protected int _zoomAmount;

        protected ICameraService _cameraService;
        public virtual Transform ZoomCenter => transform;

        private bool _activated;

        protected virtual void Awake()
        {
            _cameraService = ServiceFinder.Get<ICameraService>();
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
            if (collision.CompareTag("hero") && (!_activated || _cameraService.MainCamera.CurrentMode == CameraMode.Follow))
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

        public virtual void Activate(IActivator activator = default)
        {
            if (_cameraService.MainCamera.CurrentMode != CameraMode.Follow)
                return;

            _cameraService.MainCamera.Zoom(ZoomType.Instant, _zoomAmount);
            _cameraService.MainCamera.SetCenterMode(ZoomCenter.position);
            _activated = true;
            transform.localScale = Vector3.one * 1.1f;
        }

        public virtual void Deactivate(IActivator activator = default)
        {
            _cameraService.MainCamera.Zoom(ZoomType.Init);
            _cameraService.MainCamera.SetFollowMode();
            _activated = false;
            transform.localScale = Vector3.one;
        }
    }
}