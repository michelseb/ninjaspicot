using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZepLink.RiceNinja.Utils
{
    public static class BaseUtils
    {
        public const int FRAME_INTERVAL = 2;
        public const int EXPENSIVE_FRAME_INTERVAL = 3;

        public static int GetRandomInt()
        {
            return new System.Random().Next(0, int.MaxValue);
        }

        public static bool CoinFlip(float pound = .5f)
        {
            return Random.value < pound;
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
}
