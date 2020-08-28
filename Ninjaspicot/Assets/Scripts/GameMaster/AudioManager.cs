using System;
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
    private static AudioManager _instance;
    public static AudioManager Instance { get { if (_instance == null) _instance = FindObjectOfType<AudioManager>(); return _instance; } }

    public AudioClip FindByName(string name)
    {
        return _audios.FirstOrDefault(audio => audio.Name == name)?.Clip;
    }

    public void PlaySound (AudioSource source, string clip, float volume = 1)
    {
        source.PlayOneShot(FindByName(clip), volume);
    }

}
