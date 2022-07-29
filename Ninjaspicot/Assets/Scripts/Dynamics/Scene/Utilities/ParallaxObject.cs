using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.ServiceLocator.Services;
using ZepLink.RiceNinja.ServiceLocator.Services.Impl;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities
{
    public class ParallaxObject : Dynamic
    {
        [SerializeField] private Color _beginColor;
        [SerializeField] private Color _endColor;
        [SerializeField] private int _depth;
        [SerializeField] private bool _randomDepth;
        [SerializeField] private bool _initSettings;
        public int Depth => _depth > 0 ? _depth : ParallaxService.MIN_DEPTH;
        public float ParallaxFactor { get; private set; }

        private SpriteRenderer _renderer;
        private IParallaxService _parallaxService;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _parallaxService = ServiceFinder.Get<IParallaxService>();
        }

        private void Start()
        {
            if (_randomDepth)
            {
                _depth = Random.Range(ParallaxService.MIN_DEPTH, ParallaxService.MAX_DEPTH + 1);
            }

            if (_initSettings)
            {
                var scaleFactor = ParallaxService.SCALE_AMPLITUDE - (ParallaxService.SCALE_AMPLITUDE * ((float)Depth / ParallaxService.MAX_DEPTH));
                ParallaxFactor = (float)Depth / ParallaxService.MAX_DEPTH;

                if (_renderer != null)
                {
                    _renderer.color = Color.Lerp(_beginColor, _endColor, 1 - scaleFactor);
                }

                var factor = 2 * Mathf.Log(Depth + 1);
                Transform.Translate(0, factor * factor, 0, Space.World);
                Transform.localScale = Transform.localScale * scaleFactor;
            }

            if (_renderer != null)
            {
                _renderer.sortingOrder = ParallaxService.MAX_DEPTH - Depth;
            }

            _parallaxService.Add(this);
        }

        public void SetParallaxFactor(float factor)
        {
            ParallaxFactor = factor;
        }

    }
}