using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Helpers
{
    public class Interpolation<T> where T : struct
    {
        public T Origin { get; }
        public T Target { get; }
        public float Duration { get; }

        public Interpolation(T origin, T target, float duration)
        {
            Origin = origin;
            Target = target;
            Duration = duration;
        }
    }

    public static class InterpolationHelper
    {
        public static void Lerp<T>(IList<Interpolation<T>> interpolations) where T : struct
        {
            switch (typeof(T).Name)
            {
                case "Vector3":
                    ServiceFinder.Instance.Get<ICoroutineService>().StartCoroutine(LerpVector((IList<Interpolation<Vector3>>)interpolations));
                    break;

                case "Quaternion":
                    break;

                case "float":
                    break;

                case "Color":
                    break;
            }
        }

        private static IEnumerator LerpVector(IList<Interpolation<Vector3>> interpolations)
        {
            float t = 0f;

            while (t < 1)
            {


                t += Time.deltaTime;
                yield return null;
            }

        }
    }
}
