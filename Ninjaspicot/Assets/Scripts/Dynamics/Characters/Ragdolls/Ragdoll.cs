using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;

namespace ZepLink.RiceNinja.Dynamics.Characters
{
    public class Ragdoll : Dynamic
    {
        protected Rigidbody2D[] _chunks;
        protected const float EXPLOSION_STRENGTH = 3f;

        private void Awake()
        {
            _chunks = GetComponentsInChildren<Rigidbody2D>();
        }


        public void Activate(Vector2 origin)
        {
            foreach (var chunk in _chunks)
            {
                chunk.AddForce((chunk.position - origin).normalized * EXPLOSION_STRENGTH, ForceMode2D.Impulse);
            }
        }
    }
}