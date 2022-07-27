using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Scenery.Zones
{
    public class HidingZone : Dynamic, IFocusable
    {
        public bool IsSilent => true;

        public bool Active { get; set; }
        public bool Taken => !Active;

        private void Awake()
        {
            ActivateLocation();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out ISeeable seeable))
                return;

            DeactivateLocation();
            seeable.Hide();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out ISeeable seeable))
                return;

            ActivateLocation();
            seeable.Reveal();
        }

        private void ActivateLocation()
        {
            Active = true;
        }

        private void DeactivateLocation()
        {
            Active = false;
        }
    }
}