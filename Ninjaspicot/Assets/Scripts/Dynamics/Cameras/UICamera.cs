using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;

namespace ZepLink.RiceNinja.Dynamics.Cameras
{
    public class UICamera : Dynamic, ICamera
    {
        [SerializeField] private Animator _zoneChanger;
        public Camera Camera { get; private set; }
        public Canvas Canvas { get; private set; }

        private static UICamera _instance;
        public static UICamera Instance { get { if (_instance == null) _instance = FindObjectOfType<UICamera>(); return _instance; } }
        public float Size => Camera.orthographicSize;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            Canvas = GetComponentInChildren<Canvas>();
        }

        public void CameraFade()
        {
            _zoneChanger.SetTrigger("LevelEnd");
        }

        public void CameraAppear()
        {
            _zoneChanger.SetTrigger("LevelStart");
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPoint)
        {
            return Camera.ScreenToWorldPoint(screenPoint);
        }
    }
}