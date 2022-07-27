using System.Collections;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Cameras
{
    public class MainCamera : Dynamic, ICamera
    {
        [SerializeField] private int _beginZoom;

        private Vector3 _centerPos;
        private Vector3 _velocity;

        private const float ZOOM_SPEED = 2f;
        private const float FOLLOW_DELAY = 0.8f;
        private const float OFFSET_ADJUST_DELAY = 1.8f;

        private Camera _camera;
        public Camera Camera { get { if (BaseUtils.IsNull(_camera)) _camera = GetComponent<Camera>(); return _camera; } }

        public float Size { get { return Camera.orthographicSize; } private set { Camera.orthographicSize = value; } }

        private Transform _parentTransform;
        public override Transform Transform { get { if (BaseUtils.IsNull(_parentTransform)) _parentTransform = transform.parent.transform; return _parentTransform; } }
        public CameraMode CurrentMode { get; private set; }

        public ITracker CurrentTracker { get; private set; }

        private ICoroutineService _coroutineService;

        private float _screenRatio;
        private float _initialCamSize;

        private void Awake()
        {
            _coroutineService = ServiceFinder.Get<ICoroutineService>();

            Screen.orientation = ScreenOrientation.LandscapeLeft;

            _screenRatio = (float)Screen.height / Screen.width * .5f;
            _initialCamSize = 200f * _screenRatio;
        }

        private void Start()
        {
            _velocity = Vector3.zero;
            InstantZoom(_beginZoom);
            //SetFollowMode(_tracker);
            Zoom(ZoomType.Intro);
        }

        private void Update()
        {
            switch (CurrentMode)
            {
                case CameraMode.Follow:
                    Follow();
                    break;

                case CameraMode.Center:
                    Center(_centerPos, CurrentTracker);
                    break;
            }
        }

        //private void Update()
        //{
        //    if (_hero == null)
        //    {
        //        _hero = Hero.Instance;
        //        _heroTransform = _hero?.Transform;
        //        _heroStickiness = _hero?.Stickiness;
        //        ParentTransform.position = _heroTransform.position;
        //        var background = FindObjectOfType<Background>();
        //        if (background != null)
        //        {
        //            background.CenterBackground();
        //        }
        //    }
        //}

        private void Follow(ITracker tracker)
        {
            //float speed;

            //if (_heroStickiness.Walking || !_heroStickiness.Attached)
            //{
            //    speed = FOLLOW_DELAY;
            //}
            //else
            //{
            //    _normalOffset = Quaternion.Euler(0, 0, 90) * _heroStickiness.CollisionNormal * 25;
            //    speed = OFFSET_ADJUST_DELAY;
            //}
            ////Debug.Log(_normalOffset);
            //if (!_heroStickiness.Attached || _normalOffset.magnitude > 0 && Vector3.Dot(_normalOffset, Quaternion.Euler(0, 0, 90) * _heroStickiness.CollisionNormal * 25) < .5f)
            //{
            //    _normalOffset = Vector3.zero;
            //}

            var offset = Quaternion.Euler(0, 0, 90) * tracker.NormalVector * 25;

            Transform.position = Vector3.SmoothDamp(Transform.position, tracker.Transform.position + offset, ref _velocity, FOLLOW_DELAY);
        }

        private void Follow()
        {
            if (CurrentTracker == null)
                return;

            Follow(CurrentTracker);
        }

        public void MoveTo(Vector3 pos)
        {
            Transform.position = new Vector3(pos.x, pos.y, Transform.position.z);
        }

        private void Center(Vector3 centerPos, ITracker tracker)
        {
            var trackerPosition = tracker?.Transform.position ?? Vector2.zero;
            var middle = (trackerPosition + centerPos) / 2;

            Transform.position = Vector3.SmoothDamp(Transform.position, middle, ref _velocity, FOLLOW_DELAY);
        }

        public void SetFollowMode()
        {
            CurrentMode = CameraMode.Follow;
            _velocity = Vector3.zero;
        }

        public void SetCenterMode(Vector3 centerPos)
        {
            CurrentMode = CameraMode.Center;
            _centerPos = new Vector3(centerPos.x, centerPos.y, Transform.position.z);
        }

        private IEnumerator ZoomProgressive(int zoom)
        {
            float _initSize = Size;

            while (Mathf.Abs(Size - _initSize) < Mathf.Abs(zoom * _screenRatio))
            {
                Size -= zoom * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
                yield return null;
            }
        }

        private IEnumerator ReinitZoom()
        {
            var delta = _initialCamSize - Size;

            while (Mathf.Sign(delta) * (_initialCamSize - Size) > 0)
            {
                Size += delta * Time.unscaledDeltaTime * ZOOM_SPEED * _screenRatio;
                yield return null;
            }

            Size = _initialCamSize;
        }

        private IEnumerator ZoomIntro(float speed)
        {
            yield return new WaitForSecondsRealtime(2);

            while (Size > _initialCamSize)
            {
                Size -= speed;
                yield return null;
            }
            SetFollowMode();
            //_hero.Jumper.Active = true;
        }

        private void InstantZoom(float zoom)
        {
            Size = _initialCamSize + zoom * _screenRatio;
        }

        public void Zoom(ZoomType type, int zoomAmount = 0)
        {
            StopAllCoroutines();

            switch (type)
            {
                case ZoomType.Progressive:
                    _coroutineService.StartCoroutine(ZoomProgressive(zoomAmount));
                    break;

                case ZoomType.Instant:
                    InstantZoom(zoomAmount);
                    break;

                case ZoomType.Intro:
                    _coroutineService.StartCoroutine(ZoomIntro(ZOOM_SPEED));
                    break;

                case ZoomType.Init:
                    _coroutineService.StartCoroutine(ReinitZoom());
                    break;
            }
        }

        public void Shake(float duration, float strength)
        {
            _coroutineService.StartCoroutine(DoShake(duration, strength));
        }

        private IEnumerator DoShake(float duration, float strength)
        {
            var pos = Transform.localPosition;

            float ellapsed = 0;

            while (ellapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * strength;
                float y = Random.Range(-1f, 1f) * strength;

                Transform.localPosition = new Vector3(x, y, pos.z);

                ellapsed += Time.deltaTime;
                yield return null;
            }

            Transform.localPosition = pos;
        }

        public void SetTracker(ITracker tracker)
        {
            CurrentTracker = tracker;
        }

        public void SetMode(CameraMode cameraMode)
        {
            CurrentMode = cameraMode;
        }
    }
}