using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class CheckPoint : SceneryElement, IPoolable
    {
        public bool Attained { get; private set; }

        public void Attain()
        {
            Attained = true;
        }

        public override void Pool(Vector3 position, Quaternion rotation, float size = 1)
        {
            Transform.position = position;
        }

        public void Sleep()
        {
            gameObject.SetActive(false);
        }

        public void Wake()
        {
            gameObject.SetActive(true);
        }
    }
}