using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using ZepLink.RiceNinja.ServiceLocator;
using ZepLink.RiceNinja.ServiceLocator.Services;

namespace ZepLink.RiceNinja.Helpers
{
    //public abstract class Interpolation
    //{
    //    public float Duration { get; }

    //    public Interpolation(float duration)
    //    {
    //        Duration = duration;
    //    }

    //    internal abstract void Process(ref float elapsed);
    //    internal abstract void Complete();
    //}

    public abstract class Interpolation<T, U> where T : Component where U : struct
    {
        public T Reference { get; }
        public U Value { get; protected set; }
        public U Origin { get; }
        public U Target { get; }
        public float Duration { get; }

        public Interpolation(T reference, U origin, U target, float duration)
        {
            Reference = reference;
            Value = origin;
            Origin = origin;
            Target = target;
            Duration = duration;
        }

        public Interpolation(T reference, U value, U origin, U target, float duration)
        {
            Reference = reference;
            Value = value;
            Origin = origin;
            Target = target;
            Duration = duration;
        }

        internal abstract void Process(ref float elapsed);
        internal abstract void Apply();

        internal virtual void Complete()
        {
            Value = Target;
        }
    }

    public class TransformPositionInterpolation : Interpolation<Transform, Vector3>
    {
        public float Speed { get; }

        public TransformPositionInterpolation(Transform transform, Vector3 origin, Vector3 target, float duration, float speed) : base(transform, origin, target, duration)
        {
            Speed = speed;
        }

        public TransformPositionInterpolation(Transform transform, Vector3 origin, Vector3 target, float duration) : this(transform, origin, target, duration, 1) { }
        public TransformPositionInterpolation(Transform transform, Vector3 target, float duration) : this(transform, transform.position, target, duration) { }
        public TransformPositionInterpolation(Transform transform, Vector3 target, float duration, float speed) : this(transform, transform.position, target, duration, speed) { }

        internal override void Process(ref float elapsed)
        {
            Value = Vector3.Lerp(Origin, Target, elapsed / Duration);
            elapsed += Time.deltaTime * Speed;
        }

        internal override void Apply()
        {
            Reference.position = Value;
        }

        internal override void Complete()
        {
            Reference.position = Target;
        }
    }

    public class LightIntensityInterpolation : Interpolation<Light2D, float>
    {
        public LightIntensityInterpolation(Light2D light, float value, float origin, float target, float duration) : base(light, value, origin, target, duration) { }

        internal override void Process(ref float elapsed)
        {
            Value = Mathf.Lerp(Origin, Target, elapsed / Duration);
            elapsed += Time.deltaTime;
        }

        internal override void Apply()
        {
            Reference.intensity = Value;
        }
    }

    public class AudioVolumeInterpolation : Interpolation<AudioSource, float>
    {
        public AudioVolumeInterpolation(AudioSource audio, float value, float origin, float target, float duration) : base(audio, value, origin, target, duration) { }
        public AudioVolumeInterpolation(AudioSource audio, float origin, float target, float duration) : this(audio, origin, origin, target, duration) { }

        internal override void Process(ref float elapsed)
        {
            Value = Mathf.Lerp(Origin, Target, elapsed / Duration);
            elapsed += Time.deltaTime;
        }

        internal override void Apply()
        {
            Reference.volume = Value;
        }
    }

    public static class InterpolationHelper<T, U> where T : Component where U : struct
    {
        public static void Execute(Interpolation<T, U> interpolation)
        {
            var coroutineService = ServiceFinder.Instance.Get<ICoroutineService>();

            coroutineService.StartCoroutine(Interpolate(interpolation));
        }

        public static void Execute(IList<Interpolation<T, U>> interpolations)
        {
            var coroutineService = ServiceFinder.Instance.Get<ICoroutineService>();

            coroutineService.StartCoroutine(InterpolateList(interpolations));
        }

        public static IEnumerator Interpolate(Interpolation<T, U> interpolation)
        {
            float elapsed = 0f;

            while (elapsed < interpolation.Duration)
            {
                interpolation.Process(ref elapsed);
                interpolation.Apply();

                yield return null;
            }

            interpolation.Complete();
        }

        public static IEnumerator InterpolateList(IList<Interpolation<T, U>> interpolations)
        {
            if (interpolations == null || interpolations.Count == 0)
                yield break;

            float[] elapsed = new float[interpolations.Count];

            // TODO => Fix : not gonna work with a while loop inside
            for (var i = 0; i < interpolations.Count; i++)
            {
                var interpolation = interpolations[i];
                var e = elapsed[i];

                while (e < interpolation.Duration)
                {
                    interpolation.Process(ref e);
                    interpolation.Apply();
                }

                yield return null;
            }

            foreach (var interpolation in interpolations)
            {
                interpolation.Complete();
            }
        }
    }
}
