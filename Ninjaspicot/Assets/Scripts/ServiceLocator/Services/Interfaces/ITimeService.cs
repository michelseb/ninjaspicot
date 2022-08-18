namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface ITimeService : IGameService
    {
        /// <summary>
        /// Slow time to default slowDown value
        /// </summary>
        void SlowDownImmediate();

        /// <summary>
        /// Slow time progressively with default values
        /// </summary>
        void SlowDownProgressive();

        /// <summary>
        /// Restores time progressively with default values
        /// </summary>
        void RestoreProgressive();

        /// <summary>
        /// SlowdownProgressive then RestoreProgressive
        /// </summary>
        void SlowDownAndRestoreProgressive();

        /// <summary>
        /// Set timescale progressively to given value within given delay
        /// </summary>
        /// <param name="timeScale"></param>
        /// <param name="delay"></param>
        void SetTimeScaleProgressive(float timeScale, float delay);

        /// <summary>
        /// Set timescale progressively with default interpolation values
        /// </summary>
        /// <param name="timeScale"></param>
        void SetTimeScaleProgressive(float timeScale);

        /// <summary>
        /// Set timescale to given value immediately
        /// </summary>
        /// <param name="timeScale"></param>
        void SetTimeScaleImmediate(float timeScale);

        /// <summary>
        /// Sets time factor to 1
        /// </summary>
        void SetNormalTime();

        /// <summary>
        /// Sets time factor to 0
        /// </summary>
        void StopTime();
    }
}