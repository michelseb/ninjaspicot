using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{
    public const int FRAME_INTERVAL = 2;
    public const int EXPENSIVE_FRAME_INTERVAL = 3;

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

    public static RaycastHit2D RayCast(Vector2 origin, Vector2 direction, float distance = 0, int ignore = 0, bool includeTriggers = false, int layer = ~0)
    {
        RaycastHit2D[] hits = RayCastAll(origin, direction, distance, ignore, includeTriggers, layer);

        if (hits.Length == 0)
            return new RaycastHit2D();

        return hits[0];
    }

    public static RaycastHit2D LineCast(Vector2 origin, Vector2 destination, int[] ignore = null, bool includeTriggers = false, string target = "", int layer = ~0)
    {
        RaycastHit2D[] hits = LineCastAll(origin, destination, ignore, includeTriggers, layer);

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

    public static RaycastHit2D[] RayCastAll(Vector2 origin, Vector2 direction, float distance = 0, int ignore = 0, bool includeTriggers = false, int layer = ~0)
    {
        var hits = distance > 0 ?
            Physics2D.RaycastAll(origin, direction, distance, layer) :
            Physics2D.RaycastAll(origin, direction, float.MaxValue, layer);

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

    public static RaycastHit2D[] LineCastAll(Vector2 origin, Vector2 destination, int[] ignore = null, bool includeTriggers = false, int layer = ~0)
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(origin, destination, layer);

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

    public static RaycastHit2D CircleCast(Vector2 origin, float radius, Vector2 destination, float distance, int[] ignore = null, bool includeTriggers = false, int layer = ~0)
    {
        RaycastHit2D[] hits = CircleCastAll(origin, radius, destination, distance, ignore, includeTriggers, layer);

        if (hits.Length == 0)
            return new RaycastHit2D();

        return hits[0];
    }


    public static RaycastHit2D[] CircleCastAll(Vector2 origin, float radius, Vector2 destination, float distance, int[] ignore = null, bool includeTriggers = false, int layer = ~0)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, radius, destination, distance, layer);

        var actualHits = new List<RaycastHit2D>();

        foreach (var hit in hits)
        {
            if (includeTriggers && ignore == null)
            {
                actualHits.Add(hit);
                continue;
            }

            if (!hit.collider.TryGetComponent(out IRaycastable raycastable))
                continue;

            if (ignore != null && ignore.Any(i => i == raycastable.Id) ||
                (!includeTriggers && hit && hit.collider.isTrigger))
                continue;

            actualHits.Add(hit);
        }

        return actualHits.ToArray();
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

    public static Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public static float GetAngleFromVector(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        return angle;
    }

    public static bool IsNull(object obj)
    {
        return obj == null || obj.Equals(null) || obj == "null";
    }

    public static Vector2 ToVector2(Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static List<T> FindObjectsOfTypeInScene<T>(string scene)
    {
        return SceneManager.GetSceneByName(scene)
            .GetRootGameObjects()
            .Select(go => go.GetComponentInChildren<T>())
            .Where(x => !IsNull(x))
            .ToList();

        //return Object.FindObjectsOfType<MonoBehaviour>()
        //    .Where(go => sceneObjectsIds.Contains(go.GetInstanceID()))
        //    .OfType<T>()
        //    .ToList();
    }
}
