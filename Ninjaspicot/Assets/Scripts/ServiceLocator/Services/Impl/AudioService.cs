using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZepLink.RiceNinja.Manageables.Audios;
using ZepLink.RiceNinja.ServiceLocator.Services.Abstract;

namespace ZepLink.RiceNinja.ServiceLocator.Services.Impl
{
    public class AudioService : CoroutineService<Guid, AudioFile>, IAudioService
    {
        private Dictionary<int, string> _playedClips = new Dictionary<int, string>();
        private AudioSource _globalAudioSource;

        public override void Init(Transform parent)
        {
            base.Init(parent);

            _globalAudioSource = ServiceObject.AddComponent<AudioSource>();
        }

        public AudioFile FindAudioByName(string name)
        {
            return Collection.FirstOrDefault(audio => audio.Name == name);
        }

        public string GetSourceClip(int sourceId)
        {
            return _playedClips.ContainsKey(sourceId) ? _playedClips[sourceId] : string.Empty;
        }


        public void PlaySound(AudioSource source, string clipName, float volume = 1)
        {
            PlaySound(source, FindAudioByName(clipName), volume);
        }

        public void PlaySound(AudioSource source, AudioClip clip, float volume = 1)
        {
            if (source == null)
                return;

            source.PlayOneShot(clip, volume);
        }

        public void PlaySound(AudioSource source, AudioFile audio, float volume = 1)
        {
            if (source == null)
                return;

            _playedClips[source.GetInstanceID()] = audio.Name;
            PlaySound(source, audio.Clip, volume);
        }

        public void PlaySound(IAudio source, string clipName, float volume = 1)
        {
            PlaySound(source.AudioSource, FindAudioByName(clipName), volume);
        }

        public void PlaySound(IAudio source, AudioFile audio, float volume = 1)
        {
            PlaySound(source.AudioSource, audio, volume);
        }

        public void SetVolumeProgressive(AudioSource source, float targetvolume, float timePeriod)
        {
            CoroutineServiceBehaviour.StartCoroutine(DoSetVolumeProgressive(source, targetvolume, timePeriod));
        }

        public void SetGlobalVolumeProgressive(float targetvolume, float timePeriod)
        {
            SetVolumeProgressive(_globalAudioSource, targetvolume, timePeriod);
        }

        private IEnumerator DoSetVolumeProgressive(AudioSource source, float targetVolume, float timePeriod)
        {
            var t = 0f;
            var initVolume = source.volume;

            while (t < timePeriod)
            {
                source.volume = Mathf.Lerp(initVolume, targetVolume, t);
                t += Time.deltaTime;
                yield return null;
            }

            source.volume = targetVolume;
        }

        public void PlayGlobal(string clipName, float volume = 1f)
        {
            PlaySound(_globalAudioSource, clipName, volume);
        }

        public void PlayGlobal(AudioClip clip, float volume = 1f)
        {
            PlaySound(_globalAudioSource, clip, volume);
        }

        public void PlayGlobal(AudioFile file, float volume = 1f)
        {
            PlayGlobal(file.Clip, volume);
        }

        public void PauseGlobal()
        {
            _globalAudioSource.Pause();
        }

        public void ResumeGlobal()
        {
            _globalAudioSource.Play();
        }
    }
}
