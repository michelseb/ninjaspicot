using System;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Audios;

namespace ZepLink.RiceNinja.ServiceLocator.Services
{
    public interface IAudioService : ICollectionService<Guid, AudioFile>
    {
        /// <summary>
        /// Get audioSource by id
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        string GetSourceClip(int sourceId);

        /// <summary>
        /// Play sound matching clipname from given audioSource at given volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clipName"></param>
        /// <param name="volume"></param>
        void PlaySound(AudioSource source, string clipName, float volume = 1);

        /// <summary>
        /// Play sound from given audioSource at given volume 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="audio"></param>
        /// <param name="volume"></param>
        void PlaySound(AudioSource source, AudioFile audio, float volume = 1);

        /// <summary>
        /// Play clip from given audioSource at given volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        void PlaySound(AudioSource source, AudioClip clip, float volume = 1);

        /// <summary>
        /// Play sound matching clipname from given audio item at given volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clipName"></param>
        /// <param name="volume"></param>
        void PlaySound(IAudio source, string clipName, float volume = 1);

        /// <summary>
        /// Play sound from given audio item at given volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clipName"></param>
        /// <param name="volume"></param>
        void PlaySound(IAudio source, AudioFile clipName, float volume = 1);

        /// <summary>
        /// Play sound from global audioSource
        /// </summary>
        void PlayGlobal(string clipName, float volume = 1);

        /// <summary>
        /// Play soundclip from global audioSource
        /// </summary>
        void PlayGlobal(AudioClip clip, float volume = 1);

        /// <summary>
        /// Play sound file from global audioSource
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="volume"></param>
        void PlayGlobal(AudioFile clip, float volume = 1);

        /// <summary>
        /// Pause global audio source
        /// </summary>
        void PauseGlobal();

        /// <summary>
        /// Resume global audio source
        /// </summary>
        void ResumeGlobal();

        /// <summary>
        /// Set volume of given audio source to target volume in given amout of seconds
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetvolume"></param>
        /// <param name="timePeriod"></param>
        void SetVolumeProgressive(AudioSource source, float targetvolume, float timePeriod);

        /// <summary>
        /// Set global volume to target volume in given amout of seconds
        /// </summary>
        /// <param name="targetVolume"></param>
        void SetGlobalVolumeProgressive(float targetVolume, float timePeriod);
    }
}