using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class CheckPoint : Dynamic
    {
        [SerializeField] private int _order;
        public bool Attained { get; private set; }
        public int Order => _order;

        public void Attain()
        {
            Attained = true;
        }
    }
}