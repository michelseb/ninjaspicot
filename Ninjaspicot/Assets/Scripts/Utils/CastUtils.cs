using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Dynamics.Interfaces;

namespace ZepLink.RiceNinja.Utils
{
    public static class CastUtils
    {
        public const int FRAME_INTERVAL = 2;
        public const int EXPENSIVE_FRAME_INTERVAL = 3;

        public static LayerMask OBSTACLES => GetMask("Obstacle");
        public static LayerMask TRIGGERS => GetMask("Trigger");

        public static RaycastHit2D BoxCast(Vector2 origine, Vector2 size, float angle, Vector2 direction, float distance, int ignore = 0, bool display = false, bool includeTriggers = false, int layer = ~0)
        {
            RaycastHit2D[] hits = BoxCastAll(origine, size, angle, direction, distance, ignore, includeTriggers, layer);

            if (hits.Length == 0)
                return new RaycastHit2D();

            //Setting up the points to draw the cast
            //Drawing the cast
            if (display)
            {
                Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
                float w = size.x * 0.5f;
                float h = size.y * 0.5f;
                p1 = new Vector2(-w, h);
                p2 = new Vector2(w, h);
                p3 = new Vector2(w, -h);
                p4 = new Vector2(-w, -h);

                Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
                p1 = q * p1;
                p2 = q * p2;
                p3 = q * p3;
                p4 = q * p4;

                p1 += origine;
                p2 += origine;
                p3 += origine;
                p4 += origine;

                Vector2 realDistance = direction.normalized * distance;
                p5 = p1 + realDistance;
                p6 = p2 + realDistance;
                p7 = p3 + realDistance;
                p8 = p4 + realDistance;


                Color castColor = hits.Count(x => !x.collider.isTrigger) > 0 ? Color.red : Color.green;
                Debug.DrawLine(p1, p2, castColor);
                Debug.DrawLine(p2, p3, castColor);
                Debug.DrawLine(p3, p4, castColor);
                Debug.DrawLine(p4, p1, castColor);

                Debug.DrawLine(p5, p6, castColor);
                Debug.DrawLine(p6, p7, castColor);
                Debug.DrawLine(p7, p8, castColor);
                Debug.DrawLine(p8, p5, castColor);

                Debug.DrawLine(p1, p5, Color.grey);
                Debug.DrawLine(p2, p6, Color.grey);
                Debug.DrawLine(p3, p7, Color.grey);
                Debug.DrawLine(p4, p8, Color.grey);
            }

            return hits[0];
        }


        public static RaycastHit2D[] BoxCastAll(Vector2 origine, Vector2 size, float angle, Vector2 direction, float distance, int ignore = 0, bool includeTriggers = false, int layer = ~0)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(origine, size, angle, direction, distance, layer);

            var actualHits = hits.Where(x => !x.collider.isTrigger && x.collider.gameObject.GetInstanceID() != ignore).ToArray();
            if (includeTriggers)
            {
                actualHits = hits.Where(x => x.collider.gameObject.GetInstanceID() != ignore).ToArray();
            }

            return actualHits;
        }

        public static RaycastHit2D RayCast(Vector2 origin, Vector2 direction, float distance = 0, int ignore = 0, bool includeTriggers = false, int layerMask = 0)
        {
            RaycastHit2D[] hits = RayCastAll(origin, direction, distance, ignore, includeTriggers, layerMask);

            if (hits.Length == 0)
                return new RaycastHit2D();

            return hits[0];
        }

        public static RaycastHit2D LineCast(Vector2 origin, Vector2 destination, int[] ignore = null, bool includeTriggers = false, string target = "", int layerMask = 0)
        {
            RaycastHit2D[] hits = LineCastAll(origin, destination, ignore, includeTriggers, layerMask);

            if (hits.Length == 0)
                return new RaycastHit2D();

            if (!string.IsNullOrEmpty(target))
            {
                var wrongHit = hits.FirstOrDefault(h => h.transform.tag != target);

                if (wrongHit)
                    return wrongHit;
            }

            return hits[0];
        }

        public static RaycastHit2D[] RayCastAll(Vector2 origin, Vector2 direction, float distance = 0, int ignore = 0, bool includeTriggers = false, int layerMask = 0)
        {
            distance = distance > 0 ? distance : float.PositiveInfinity;

            var hits = layerMask > 0 ?
                Physics2D.RaycastAll(origin, direction, distance, layerMask) :
                Physics2D.RaycastAll(origin, direction, distance);

            var actualHits = new List<RaycastHit2D>();

            foreach (var hit in hits)
            {

                if (!hit.collider.TryGetComponent(out IRaycastable raycastable))
                    continue;

                if (raycastable.Id == ignore || (!includeTriggers && hit.collider.isTrigger))
                    continue;

                actualHits.Add(hit);
            }

            return actualHits.ToArray();
        }

        public static RaycastHit2D[] LineCastAll(Vector2 origin, Vector2 destination, int[] ignore = null, bool includeTriggers = false, int layerMask = 0)
        {
            RaycastHit2D[] hits = layerMask > 0 ? Physics2D.LinecastAll(origin, destination, layerMask) : Physics2D.LinecastAll(origin, destination);

            var actualHits = new List<RaycastHit2D>();

            foreach (var hit in hits)
            {
                if (!hit.collider.TryGetComponent(out IRaycastable raycastable))
                    continue;

                if (ignore != null && ignore.Any(i => i == raycastable.Id) ||
                    (!includeTriggers && hit && hit.collider.isTrigger))
                    continue;

                actualHits.Add(hit);
            }

            return actualHits.ToArray();
        }

        public static LayerMask GetMask(params string[] masks)
        {
            LayerMask result = default;

            foreach (var mask in masks)
            {
                result |= 1 << LayerMask.NameToLayer(mask);
            }

            return result;
        }

        public static LayerMask AllButMasks(params string[] masks)
        {
            LayerMask result = default;

            foreach (var mask in masks)
            {
                result |= ~(1 << LayerMask.NameToLayer(mask));
            }

            return result;
        }
    }
}
