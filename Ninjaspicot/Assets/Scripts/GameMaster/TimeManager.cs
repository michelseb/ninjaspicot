using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

    public float slowValue = .06f, increaseValue = .05f;


    public void SlowDown()
    {
        Time.timeScale = slowValue;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = Time.timeScale;
    }

    public IEnumerator RestoreTime()
    {
        while (Time.timeScale < 1)
        {
            Time.timeScale += increaseValue * Time.unscaledDeltaTime;
            increaseValue += .1f;
            yield return null;
        }
        Time.timeScale = 1;
        increaseValue = .05f;
    }
}
