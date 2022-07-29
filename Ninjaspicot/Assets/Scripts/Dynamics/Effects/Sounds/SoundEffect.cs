using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Effects.Sounds
{
    public class SoundEffect : Dynamic, IPoolable
    {
        private float _currentLifeTime;
        private const float LIFE_TIME = 1;

        private void Start()
        {
            _currentLifeTime = LIFE_TIME;
        }

        private void Update()
        {
            _currentLifeTime -= Time.deltaTime;
            if (_currentLifeTime <= 0)
            {
                Sleep();
            }
        }

        public void Pool(Vector3 position, Quaternion rotation, float size)
        {
            Transform.localScale = Vector3.one * size;
            Transform.position = new Vector3(position.x, position.y, -5);
            Transform.rotation = rotation;
        }

        public void Sleep()
        {
            gameObject.SetActive(false);
        }

        public void Wake()
        {
            gameObject.SetActive(true);
            _currentLifeTime = LIFE_TIME;
        }

        public void DoReset()
        {
            Sleep();
        }
    }
}