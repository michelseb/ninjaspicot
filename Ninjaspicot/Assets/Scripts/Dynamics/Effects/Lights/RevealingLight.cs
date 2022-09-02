using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;
using ZepLink.RiceNinja.Utils;

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
            var id = collision.GetInstanceID();

            if (!TryGetSeeable(id, collision, out ISeeable seeable))
                return;

            if (CastUtils.LineCast(Transform.position, collision.transform.position, layerMask: CastUtils.OBSTACLES))
            {
                if (_seeables.ContainsKey(id))
                {
                    seeable.Hide();
                    _seeables.Remove(id);
                }

                return;
            }

            if (_seeables.ContainsKey(id))
                return;

            _seeables.Add(id, seeable);
            seeable.Reveal();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            var id = collision.GetInstanceID();

            if (!TryGetSeeable(id, collision, out ISeeable seeable))
                return;

            seeable.Hide();
            _seeables.Remove(id);
        }

        private bool TryGetSeeable(int id, Collider2D collision, out ISeeable seeable)
        {
            seeable = default;

            if (!collision.CompareTag("hero"))
                return false;

            if (_seeables.ContainsKey(id))
            {
                seeable = _seeables[id];
                return true;
            }

            return collision.TryGetComponent(out seeable);
        }
    }
}
