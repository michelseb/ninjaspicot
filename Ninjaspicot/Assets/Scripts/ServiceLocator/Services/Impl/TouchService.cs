using System.Collections;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Inputs;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Helpers;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public enum TouchType
    {
        Left,
        Right
    }

    public class TouchService : PoolService<Joystick>, ITouchService
    {
        private bool _mobileTouch;
        private Joystick _joystick1;
        private Joystick _joystick2;

        public MonoBehaviour ServiceBehaviour { get; private set; }

        public bool LeftSideTouching => IsTouching(TouchType.Left);
        public bool RightSideTouching => IsTouching(TouchType.Right);
        public bool LeftSideTouchStarting => LeftSideTouching && !_leftTouchInitialized;
        public bool LeftSideTouchEnding => !LeftSideTouching && _leftTouchInitialized;
        public bool Touching => LeftSideTouching || RightSideTouching;
        public bool LeftSideTouchDragging => _leftSideDragging || (_joystick1 != null && _joystick1.Direction.magnitude > .2f);
        public bool RightSideTouchDragging => _joystick2 != null && _joystick2.Direction.magnitude > .2f;
        public bool RightSideTouchStarting => RightSideTouching && !_rightTouchInitialized;
        public bool RightSideTouchEnding => !RightSideTouching && _rightTouchInitialized;
        public bool DoubleTouching => RightSideTouching && LeftSideTouching;
        public Vector3 LeftDragDirection => _joystick1?.Direction ?? Vector3.zero;
        public Vector3 RightDragDirection => _joystick2?.Direction ?? Vector3.zero;
        public IControllable CurrentControllable { get; private set; }

        private Vector3? _leftTouch => GetTouch(TouchType.Left);
        private Vector3? _rightTouch => GetTouch(TouchType.Right);

        private ICameraService _cameraService;

        private bool _leftTouchInitialized;
        private bool _rightTouchInitialized;
        private bool _leftSideDragging;

        public TouchService(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            ServiceBehaviour = ServiceObject.AddComponent<ServiceBehaviour>();
            ServiceBehaviour.StartCoroutine(HandleEvents());
        }

        private IEnumerator HandleEvents()
        {
            while (true)
            {
                HandleLeftSideTouchInitEvent();
                HandleRightSideTouchInitEvent();
                if (!HandleRightSideTouchEvent())
                {
                    HandleLeftSideTouchEvent(DoubleTouching);
                }
                HandleLeftSideTouchEndEvent();
                HandleRightSideTouchEndEvent(DoubleTouching);

                yield return null;
            }
        }

        private bool HandleLeftSideTouchInitEvent()
        {
            if (!LeftSideTouchStarting)
                return false;

            var mainCamera = _cameraService.MainCamera;

            CurrentControllable?.OnLeftSideTouchInit();

            var touchPos = mainCamera.ScreenToWorldPoint(_leftTouch.Value);
            _joystick1 = PoolByName("LeftJoystick", touchPos);
            _joystick1.OnPointerDown();
            _leftTouchInitialized = true;

            return true;
        }

        private bool HandleLeftSideTouchEvent(bool doubleTouching)
        {
            if (!LeftSideTouching)
                return false;

            CurrentControllable?.OnLeftSideTouch();

            // If already dragging
            if (!LeftSideTouchStarting)
            {
                _joystick1.Drag(_leftTouch.Value, true);
            }

            return HandleLeftSideTouchDragEvent(doubleTouching);
        }

        private bool HandleLeftSideTouchDragEvent(bool doubleTouching)
        {
            if (!LeftSideTouchDragging)
                return false;

            _leftSideDragging = true;

            if (doubleTouching)
            {
                CurrentControllable?.OnDoubleTouchLeftSideDrag(LeftDragDirection);
            }
            else
            {
                CurrentControllable?.OnLeftSideDrag();
            }

            return true;
        }

        private bool HandleLeftSideTouchEndEvent()
        {
            if (!LeftSideTouchEnding)
                return false;

            _joystick1.StartFading();
            _joystick1.OnPointerUp();
            _leftSideDragging = false;
            _leftTouchInitialized = false;

            CurrentControllable?.OnLeftSideTouchEnd();

            return true;
        }

        private bool HandleRightSideTouchInitEvent()
        {
            if (!RightSideTouchStarting)
                return false;

            var mainCamera = _cameraService.MainCamera;

            CurrentControllable?.OnRightSideTouchInit();

            var touchPos = mainCamera.ScreenToWorldPoint(_rightTouch.Value);
            _joystick2 = PoolByName("RightJoystick", touchPos);
            _joystick2.OnPointerDown();
            _rightTouchInitialized = true;


            return true;
        }

        private bool HandleRightSideTouchEvent()
        {
            if (!RightSideTouching)
                return false;

            CurrentControllable?.OnRightSideTouch();

            if (!RightSideTouchStarting)
            {
                _joystick2.Drag(_rightTouch.Value);
            }

            return HandleRightSideTouchDragEvent(DoubleTouching);
        }

        private bool HandleRightSideTouchDragEvent(bool doubleTouching)
        {
            if (!RightSideTouchDragging)
                return false;

            if (doubleTouching)
            {
                CurrentControllable?.OnDoubleTouchRightSideDrag(RightDragDirection);
            }
            else
            {
                CurrentControllable?.OnRightSideDrag(RightDragDirection);
            }

            return true;
        }

        private bool HandleRightSideTouchEndEvent(bool doubleTouching)
        {
            if (!RightSideTouchEnding)
                return false;

            if (doubleTouching)
            {
                CurrentControllable?.OnDoubleTouchRightSideDragEnd(RightDragDirection);
            }
            else
            {
                CurrentControllable?.OnRightSideDragEnd(RightDragDirection);
            }

            _joystick2.StartFading();
            _joystick2.OnPointerUp();
            _rightTouchInitialized = false;

            return true;
        }

        private Vector3? GetTouch(TouchType touchType)
        {
            if (!_mobileTouch && Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (IsTouching(touchType))
                    return Input.mousePosition;

                return null;
            }

            var touches = Input.touches;
            if (touches.Count() == 0)
                return null;

            foreach (var touch in touches)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended)
                    continue;

                if (touchType == TouchType.Left && touch.position.x <= Screen.width / 2 || touchType == TouchType.Right && touch.position.x > Screen.width / 2)
                    return touch.position;
            }

            return null;
        }

        private bool IsTouching(TouchType touchType)
        {
            if (!_mobileTouch && Application.platform == RuntimePlatform.WindowsEditor)
                return touchType == TouchType.Left ? Input.GetButton("Fire1") : Input.GetButton("Fire2");

            return touchType == TouchType.Left ? _leftTouch != null : _rightTouch != null;
        }

        public void SetControllable(IControllable controllable)
        {
            CurrentControllable = controllable;
        }
    }
}