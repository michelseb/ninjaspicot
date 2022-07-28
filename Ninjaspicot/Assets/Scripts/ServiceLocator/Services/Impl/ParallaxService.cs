using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Scenery.Utilities;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class ParallaxService : CollectionService<ParallaxObject>, IParallaxService
    {
        private readonly ICameraService _cameraService;

        private Vector3 _previousCameraPosition;

        public const int MIN_DEPTH = 5;
        public const int MAX_DEPTH = 20;
        public const float PARALLAX_FACTOR = 1.5f;
        public const float SCALE_AMPLITUDE = 1;

        public ParallaxService(ICameraService cameraBehaviour)
        {
            _cameraService = cameraBehaviour;
        }

        public override void Init(Transform parent)
        {
            base.Init(parent);

            _previousCameraPosition = _cameraService.MainCamera.Transform.position;
        }

        private void Update()
        {
            var deltaPosition = _cameraService.MainCamera.Transform.position - _previousCameraPosition;

            if (deltaPosition.magnitude == 0)
                return;

            OperateParallax(deltaPosition);

            _previousCameraPosition = _cameraService.MainCamera.Transform.position;
        }

        public void AddObject(ParallaxObject parallaxObject)
        {
            if (Collection.Contains(parallaxObject))
                return;

            Collection.Add(parallaxObject);
        }

        private void OperateParallax(Vector3 delta)
        {
            //_parallaxObjects.RemoveAll(o => o == null);

            foreach (var parallaxObject in Collection)
            {
                if (parallaxObject == null)
                {
                    Collection.Remove(parallaxObject);
                    continue;
                }

                parallaxObject.transform.position += new Vector3(delta.x, delta.y, 0) * parallaxObject.ParallaxFactor;
            }
        }
    }
}