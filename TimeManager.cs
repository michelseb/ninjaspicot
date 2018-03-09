using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public bool activated;
    public float increaseValue = .05f;

    private void Start()
    {
        activated = true;
    }

    public void SlowDown(float slowValue)
    {
        if (activated == false)
        {
            return;
        }
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

    public IEnumerator RestoreTime()
    {
        while (Time.timeScale < 1)
        {
            Time.timeScale += increaseValue * Time.unscaledDeltaTime;
            increaseValue += .1f;
            Time.fixedDeltaTime = Time.timeScale * .02f;
            yield return null;
        }
        Time.timeScale = 1;
        increaseValue = .05f;
    }

    public void NormalTime()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
