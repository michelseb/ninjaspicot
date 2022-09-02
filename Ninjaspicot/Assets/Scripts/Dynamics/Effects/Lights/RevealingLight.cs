using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Dynamics.Effects.Lights
{
    public class RevealingLight : LightEffect
    {
        private IDictionary<int, ISeeable> _seeables = new Dictionary<int, ISeeable>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnTriggerStay2D(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!TryGetSeeable(collision, out ISeeable seeable, true))
                return;

            seeable.Reveal();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!TryGetSeeable(collision, out ISeeable seeable, false))
                return;

            seeable.Hide();
        }

        private bool TryGetSeeable(Collider2D collision, out ISeeable seeable, bool add)
        {
            seeable = default;

            if (!collision.CompareTag("hero"))
                return false;

            var id = collision.GetInstanceID();

            if (_seeables.ContainsKey(id))
            {
                if (add) return false;

                seeable = _seeables[id];
            }
            else
            {
                if (!collision.TryGetComponent(out seeable))
                    return false;

                if (add) _seeables.Add(id, seeable);
            }

            if (!add) _seeables.Remove(id);

            return true;
        }
    }
}
