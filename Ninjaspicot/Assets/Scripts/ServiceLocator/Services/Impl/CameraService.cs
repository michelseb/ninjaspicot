using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public enum CameraType
    {
        Main = 0,
        Ui = 1
    }

    public class CameraService : CollectionService<int, ICamera>, ICameraService
    {
        private GameObject _camerasContainer;
        public GameObject CamerasContainer { get { if (BaseUtils.IsNull(_camerasContainer)) _camerasContainer = GameObject.Find("Cameras"); return _camerasContainer; } }

        private MainCamera _mainCamera;
        public MainCamera MainCamera { get { if (BaseUtils.IsNull(_mainCamera)) _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCamera>(); return _mainCamera; } }

        private UICamera _uiCamera;
        public UICamera UiCamera { get { if (BaseUtils.IsNull(_uiCamera)) _uiCamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<UICamera>(); return _uiCamera; } }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            Add(MainCamera);
            Add(UiCamera);
        }
    }
}