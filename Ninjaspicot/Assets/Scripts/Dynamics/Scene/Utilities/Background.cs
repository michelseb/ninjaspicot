using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities
{
    public class Background : Dynamic
    {
        [SerializeField] private Sprite _image;
        [SerializeField] private float _depth;

        private List<RectTransform> _images;
        private Transform _cameraPosition;
        private Material _litMaterial;
        private ICameraService _cameraService;

        private void Start()
        {
            _cameraService = ServiceFinder.Get<ICameraService>();

            _litMaterial = Resources.Load<Material>("Sprite-Lit-Default");
            _cameraPosition = _cameraService.MainCamera.Transform;

            _images = new List<RectTransform>
            {
                CreateImage(_image, new Vector2(0, 0)),
                CreateImage(_image, new Vector2(1, 0)),
                CreateImage(_image, new Vector2(0, 1)),
                CreateImage(_image, new Vector2(1, 1))
            };
        }

        private void Update()
        {
            if (Time.frameCount % BaseUtils.EXPENSIVE_FRAME_INTERVAL == 0)
            {
                CalculateImgPositions();
            }
        }

        public void CenterBackground()
        {
            foreach (var image in _images)
            {
                image.position = new Vector3(_cameraPosition.position.x, _cameraPosition.position.y, 10);
            }
        }

        private RectTransform CreateImage(Sprite sprite, Vector2 pivot)
        {
            var imageObject = new GameObject("backgroundImage", typeof(Image));
            var img = imageObject.GetComponent<Image>();
            img.sprite = sprite;
            img.material = _litMaterial;

            imageObject.AddComponent<ParallaxObject>().SetParallaxFactor(_depth);

            var rectTransform = imageObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width / 6, Screen.height / 6);
            rectTransform.pivot = pivot;
            rectTransform.parent = transform;

            return rectTransform;
        }

        private void CalculateImgPositions()
        {
            _images.ForEach(img =>
            {
                if (img.position.x - _cameraPosition.position.x > img.rect.width / 2)
                {
                    img.position += Vector3.left * img.rect.width;
                }
                else if (_cameraPosition.position.x - img.position.x > img.rect.width / 2)
                {
                    img.position += Vector3.right * img.rect.width;
                }
                else if (img.position.y - _cameraPosition.position.y > img.rect.height / 2)
                {
                    img.position += Vector3.down * img.rect.height;
                }
                else if (_cameraPosition.position.y - img.position.y > img.rect.height / 2)
                {
                    img.position += Vector3.up * img.rect.height;
                }
            });
        }
    }
}