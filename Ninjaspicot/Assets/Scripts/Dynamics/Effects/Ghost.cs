using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

namespace ZepLink.RiceNinja.Dynamics.Effects
{
    public class Ghost : Dynamic, IPoolable
    {
        [SerializeField] private float _disappearingSpeed;
        [SerializeField] private float _initAlpha;
        [SerializeField] private Color _endColor;

        private SpriteRenderer _renderer;
        private Vector3 _initSize;
        private Color _initColor;

        private void Awake()
        {
            _initColor = ColorUtils.GetColor(CustomColor.NightBlue, .5f);
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            var color = _renderer.color;

            if (color.a <= .01f)
            {
                Sleep();
            }
            else
            {
                _renderer.color = Color.Lerp(color, _endColor, Time.deltaTime * _disappearingSpeed);
            }
        }


        public void Pool(Vector3 position, Quaternion rotation, float size)
        {
            InitColor();
            Transform.position = position;
            Transform.rotation = rotation;
            _initSize = Transform.localScale;
            Transform.localScale = new Vector3(_initSize.x * size, _initSize.y * size);
        }

        public void Sleep()
        {
            gameObject.SetActive(false);

            if (_initSize != Vector3.zero)
            {
                Transform.localScale = _initSize;
            }
        }

        public void Wake()
        {
            gameObject.SetActive(true);
            InitColor();
        }

        private void InitColor()
        {
            _renderer.color = _initColor;
        }

        public void DoReset()
        {
            Sleep();
        }
    }
}