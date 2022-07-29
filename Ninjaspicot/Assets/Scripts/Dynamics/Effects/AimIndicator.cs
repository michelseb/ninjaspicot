using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Effects
{
    public class AimIndicator : Dynamic, IPoolable
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Update()
        {
            Transform.Rotate(0, 0, 1);
        }

        public virtual void Wake()
        {
            gameObject.SetActive(true);
        }

        public virtual void Sleep()
        {
            gameObject.SetActive(false);
        }

        public void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
            Transform.position = position;
            Transform.localScale = Vector3.one * size;
        }

        public void DoReset()
        {
            Sleep();
        }
    }
}
