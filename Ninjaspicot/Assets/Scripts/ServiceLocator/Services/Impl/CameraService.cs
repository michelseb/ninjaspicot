using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Cameras;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public enum CameraType
    {
        Main = 0,
        Ui = 1
    }

    public class CameraService : CollectionService<ICamera>, ICameraService
    {
        private MainCamera _mainCamera;
        public MainCamera MainCamera { get { if (BaseUtils.IsNull(_mainCamera)) _mainCamera = Object.FindObjectOfType<MainCamera>(); return _mainCamera; } }

        private UICamera _uiCamera;
        public UICamera UiCamera { get { if (BaseUtils.IsNull(_uiCamera)) _uiCamera = Object.FindObjectOfType<UICamera>(); return _uiCamera; } }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            Add(MainCamera);
            Add(UiCamera);
        }
    }
}