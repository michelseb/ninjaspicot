using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utils
{
    public static RaycastHit2D BoxCast(Vector2 origine, Vector2 size, float angle, Vector2 direction, float distance, int ignore = 0, bool display = false, bool includeTriggers = false, int layer = ~0)
    {
        RaycastHit2D[] hits = BoxCastAll(origine, size, angle, direction, distance, ignore, includeTriggers, layer);

        //Setting up the points to draw the cast
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

        //Drawing the cast
        if (display)
        {
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


        RaycastHit2D hit = new RaycastHit2D();
        if (hits.Length > 0)
        {
            var dist = float.PositiveInfinity;
            foreach (var actualHit in hits)
            {
                var newDist = Vector3.Distance(origine, actualHit.collider.transform.position);
                if (newDist < dist)
                {
                    hit = actualHit;
                    dist = newDist;
                }
            }
            Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
            return hit;
        }

        return hit;
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

    public static RaycastHit2D RayCast(Vector2 origin, Vector2 direction, float distance = 0, int ignore = 0, bool includeTriggers = false)
    {
        RaycastHit2D[] hits = RayCastAll(origin, direction, distance, ignore, includeTriggers);

        RaycastHit2D hit = new RaycastHit2D();
        if (hits.Length > 0)
        {
            var dist = float.PositiveInfinity;
            foreach (var actualHit in hits)
            {
                var newDist = Vector3.Distance(origin, actualHit.collider.transform.position);
                if (newDist < dist)
                {
                    hit = actualHit;
                    dist = newDist;
                }
            }
        }

        return hit;
    }

    public static RaycastHit2D LineCast(Vector2 origin, Vector2 destination, int ignore = 0, bool includeTriggers = false)
    {
        RaycastHit2D[] hits = LineCastAll(origin, destination, ignore, includeTriggers);

        RaycastHit2D hit = new RaycastHit2D();
        if (hits.Length > 0)
        {
            var dist = float.PositiveInfinity;
            foreach (var actualHit in hits)
            {
                var newDist = Vector3.Distance(origin, actualHit.collider.transform.position);
                if (newDist < dist)
                {
                    hit = actualHit;
                    dist = newDist;
                }
            }
        }

        return hit;
    }

    public static RaycastHit2D[] RayCastAll(Vector2 origin, Vector2 direction, float distance = 0, int ignore = 0, bool includeTriggers = false)
    {
        RaycastHit2D[] hits;

        if (distance > 0)
        {
            hits = Physics2D.RaycastAll(origin, direction, distance);
        }
        else
        {
            hits = Physics2D.RaycastAll(origin, direction);
        }

        var actualHits = new List<RaycastHit2D>();

        foreach (var hit in hits)
        {
            var raycastable = hit.collider.GetComponent<IRaycastable>();

            if (raycastable == null ||
                raycastable.Id == ignore ||
                (!includeTriggers && hit.collider.isTrigger))
                continue;

            actualHits.Add(hit);
        }

        return actualHits.ToArray();
    }

    public static RaycastHit2D[] LineCastAll(Vector2 origin, Vector2 destination, int ignore = 0, bool includeTriggers = false)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(origin, destination);

        var actualHits = new List<RaycastHit2D>();

        foreach (var hit in hits)
        {
            var raycastable = hit.collider.GetComponent<IRaycastable>();

            if (raycastable == null ||
                raycastable.Id == ignore ||
                (!includeTriggers && hit.collider.isTrigger))
                continue;

            actualHits.Add(hit);
        }

        return actualHits.ToArray();
    }

    public static RaycastHit2D[] TypeAtPos(Vector3 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return null;
        var rayCasts = Physics2D.RaycastAll(pos, Vector2.zero);
        if (!rayCasts.Any(x => x))
            return null;
        return rayCasts;
    }

    public static bool CoinFlip(float pound = .5f)
    {
        return UnityEngine.Random.value < pound;
    }

    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }
}
