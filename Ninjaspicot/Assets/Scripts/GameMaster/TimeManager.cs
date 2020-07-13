using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour, IActivable
{
    public float TimeScale => Time.timeScale;

    private bool _active;

    private static TimeManager _instance;
    public static TimeManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TimeManager>(); return _instance; } }

    private const float INCREASE_INTERPOLATION = .5f;
    private const float DECREASE_INTERPOLATION = .8f;
    private const float TIME_SLOW = .05f;

    private void Start()
    {
        Activate();
    }

    public void SlowDown(float slowValue = TIME_SLOW)
    {
        if (!_active || Time.timeScale <= slowValue)
            return;

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
    }

    private IEnumerator RestoreTime()
    {
        yield return new WaitForSecondsRealtime(.2f);

        while (Time.timeScale < 1)
        {
            SetTimeScale(Time.timeScale + INCREASE_INTERPOLATION * Time.unscaledDeltaTime);
            yield return null;
        }

        Time.timeScale = 1;
    }

    public void StartSlowDownProgressive(float value)
    {
        StartCoroutine(SlowDownProgressive(value));
    }

    public void StartTimeRestore()
    {
        StartCoroutine(RestoreTime());
    }

    public void SetNormalTime()
    {
        SetTimeScale(1);
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
