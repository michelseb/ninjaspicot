using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    private bool _active;
    private float _increaseValue;

    public float TimeScale => Time.timeScale;

    private static TimeManager _instance;
    public static TimeManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TimeManager>(); return _instance; } }

    private void Start()
    {
        _active = true;
        _increaseValue = .5f;
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
            Time.timeScale += _increaseValue * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            yield return null;
        }

        Time.timeScale = 1;
    }

    public void StartTimeRestore()
    {
        StartCoroutine(RestoreTime());
    }

    public void SetActive(bool active)
    {
        _active = active;
    }

    public void SetNormalTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
