using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour, IActivable
{
    public float TimeScale => Time.timeScale;

    private AudioManager _audioManager;
    private Audio _slowDown;
    private bool _active;

    private Coroutine _slowDownProgressive;

    private AudioSource _globalAudioSource;
    public AudioSource GlobalAudioSource { get { if (Utils.IsNull(_globalAudioSource)) _globalAudioSource = GetComponent<AudioSource>(); return _globalAudioSource; } }

    private AudioSource _heroAudioSource;
    public AudioSource HeroAudioSource { get { if (Utils.IsNull(_heroAudioSource)) _heroAudioSource = Hero.Instance?.GetComponent<AudioSource>(); return _heroAudioSource; } }
    private static TimeManager _instance;
    public static TimeManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TimeManager>(); return _instance; } }

    private float _globalAudioSourceInitVolume;

    private const float INCREASE_INTERPOLATION = .5f;
    private const float DECREASE_INTERPOLATION = .8f;
    private const float TIME_SLOW = .01f;
    private const float VOLUME_SLOWDOWN = .05f;


    private void Awake()
    {
        _audioManager = AudioManager.Instance;
    }

    private void Start()
    {
        _slowDown = _audioManager.FindAudioByName("SlowDown");
        _globalAudioSourceInitVolume = GlobalAudioSource.volume;

        Activate();
    }

    public void SlowDown(float slowValue = TIME_SLOW)
    {
        if (!_active || Time.timeScale <= slowValue)
            return;

        GlobalAudioSource.volume = VOLUME_SLOWDOWN;//.Pause();
        _audioManager.PlaySound(HeroAudioSource, _slowDown);
        SetTimeScale(slowValue);
    }

    private IEnumerator SlowDownProgressive(float slowValue)
    {
        while (Time.timeScale > slowValue)
        {
            SetTimeScale(Time.timeScale - DECREASE_INTERPOLATION * Time.unscaledDeltaTime);
            yield return null;
        }

        Time.timeScale = slowValue;
        _slowDownProgressive = null;
    }

    private IEnumerator RestoreTime()
    {
        yield return new WaitForSecondsRealtime(.2f);

        while (Time.timeScale < 1)
        {
            SetTimeScale(Time.timeScale + INCREASE_INTERPOLATION * Time.unscaledDeltaTime);
            yield return null;
        }

        SetNormalTime();
    }

    public void StartSlowDownProgressive(float value)
    {
        _slowDownProgressive = StartCoroutine(SlowDownProgressive(value));
    }

    public void StartTimeRestore()
    {
        StartCoroutine(RestoreTime());
    }

    public void SetNormalTime()
    {
        if (_slowDownProgressive != null)
        {
            StopCoroutine(_slowDownProgressive);
        }

        SetTimeScale(1);

        if (HeroAudioSource != null && _audioManager.GetSourceClip(HeroAudioSource.GetInstanceID()) == "SlowDown")
        {
            HeroAudioSource.Stop();
        }

        if (GlobalAudioSource != null && GlobalAudioSource.volume < _globalAudioSourceInitVolume)
        {
            _audioManager.IncreaseVolumeProgressive(GlobalAudioSource, VOLUME_SLOWDOWN, _globalAudioSourceInitVolume);
        }
    }

    public void StopTime()
    {
        SetTimeScale(0);
    }

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
    }

    private void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
