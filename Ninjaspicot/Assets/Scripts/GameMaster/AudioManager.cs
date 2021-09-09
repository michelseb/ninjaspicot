using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Audio
{
    public string Name;
    public AudioClip Clip;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<Audio> _audios;
    public List<Audio> Audios => _audios;

    private Dictionary<int, string> _playedClips = new Dictionary<int, string>();

    private static AudioManager _instance;
    public static AudioManager Instance { get { if (_instance == null) _instance = FindObjectOfType<AudioManager>(); return _instance; } }

    public Audio FindAudioByName(string name)
    {
        return _audios.FirstOrDefault(audio => audio.Name == name);
    }

    public string GetSourceClip(int sourceId)
    {
        return _playedClips.ContainsKey(sourceId) ? _playedClips[sourceId] : string.Empty;
    }

    public void PlaySound(AudioSource source, string clipName, float volume = 1)
    {
        PlaySound(source, FindAudioByName(clipName), volume);
    }

    public void PlaySound(AudioSource source, Audio audio, float volume = 1)
    {
        if (source == null)
            return;

        _playedClips[source.GetInstanceID()] = audio.Name;
        source.PlayOneShot(audio.Clip, volume);
    }

    public void IncreaseVolumeProgressive(AudioSource source, float initVolume = 0f, float volume = 1f)
    {
        StartCoroutine(PlayProgressive(source, initVolume, volume));
    }

    private IEnumerator PlayProgressive(AudioSource source, float initVolume, float volume)
    {
        source.volume = initVolume;
        //source.Play();

        while (source.volume < volume)
        {
            source.volume += Time.deltaTime / 10;
            yield return null;
        }

        source.volume = volume;
    }

}
