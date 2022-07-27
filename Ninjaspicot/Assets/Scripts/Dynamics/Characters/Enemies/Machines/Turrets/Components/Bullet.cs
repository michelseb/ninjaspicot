using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Characters.Enemies.Machines.Turrets.Components
{
    public class Bullet : Dynamic, IPoolable
    {
        private LineRenderer _lineRenderer;
        public LineRenderer LineRenderer { get { if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>(); return _lineRenderer; } }

        private ParticleSystem _particleSystem;
        public ParticleSystem ParticleSystem { get { if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>(); return _particleSystem; } }


        public void Pool(Vector3 position, Quaternion rotation, float size)
        {
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
        }

        public void DoReset()
        {
            Sleep();
        }
    }
}