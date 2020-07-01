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

    private void Start()
    {
        Activate();
    }

    public void SlowDown(float slowValue)
    {
        if (!_active)
            return;

        if (Time.timeScale > slowValue)
        {
            Time.timeScale = slowValue;
        }

        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    private IEnumerator SlowDownProgressive(float slowValue)
    {
        while (Time.timeScale > slowValue)
        {
            Time.timeScale -= DECREASE_INTERPOLATION * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            yield return null;
        }

        Time.timeScale = slowValue;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    private IEnumerator RestoreTime()
    {
        yield return new WaitForSecondsRealtime(.2f);

        while (Time.timeScale < 1)
        {
            Time.timeScale += INCREASE_INTERPOLATION * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * .02f;
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
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public void Activate()
    {
        _active = true;
    }

    public void Deactivate()
    {
        _active = false;
    }
}
