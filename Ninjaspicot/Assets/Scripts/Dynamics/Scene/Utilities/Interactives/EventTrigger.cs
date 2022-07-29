using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Abstract;
using ZepLink.RiceNinja.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Utilities.Interactives
{
    public class EventTrigger : Dynamic
    {
        [SerializeField] private bool _singleTime;
        public bool SingleTime => _singleTime;


        private void OnTriggerEnter2D(Collider2D collision)
        {
            var triggerable = collision.GetComponent<ITriggerable>() ?? collision.GetComponentInParent<ITriggerable>();
            triggerable?.StartTrigger(this);
        }
    }
}