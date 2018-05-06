using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

    Coroutine ending;
    CameraBehaviour c;
    bool isExiting;
    TimeManager t;

    private void Awake()
    {
        c = FindObjectOfType<CameraBehaviour>();
        t = FindObjectOfType<TimeManager>();
    }

    private void Update()
    {
        transform.Rotate(0, 0, 120);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ninjaspicot")
        {
            if(isExiting == false)
            {
                StartCoroutine(EndAnimation());
            }
        }
        
    }



    IEnumerator EndAnimation()
    {
        c.centerX = false;
        isExiting = true;
        StartCoroutine(c.zoomIn(30));
        t.SlowDown(.1f);
        yield return new WaitForSecondsRealtime(1);
        StartCoroutine(c.zoomIn(60));
        yield return new WaitForSecondsRealtime(.2f);
        t.RestoreTime();
        ScenesManager.NextScene();
    }
}
