﻿using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    private bool _active;
    private float _timeToOpen;
    public GameObject leftDoor, rightDoor;
    Camera cam;
    GUIStyle style;
    // Use this for initialization
    void Start()
    {
        cam = FindObjectOfType<Camera>();
        style = new GUIStyle();
        style.fontSize = 15;
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            _timeToOpen -= Time.unscaledDeltaTime;
            if (_timeToOpen < 1)
            {
                StartCoroutine(Open());
                _active = false;
                _timeToOpen = 0;
            }
        }
    }

    private IEnumerator Open()
    {
        Color col = leftDoor.GetComponent<Renderer>().material.color;
        Destroy(leftDoor.GetComponent<BoxCollider2D>());
        Destroy(rightDoor.GetComponent<BoxCollider2D>());
        while (col.a > 0)
        {
            leftDoor.transform.Translate(-4 * Time.unscaledDeltaTime, 0, 0);
            rightDoor.transform.Translate(4 * Time.unscaledDeltaTime, 0, 0);
            col = leftDoor.GetComponent<Renderer>().material.color;
            col.a -= Time.deltaTime;
            leftDoor.GetComponent<Renderer>().material.color = col;
            rightDoor.GetComponent<Renderer>().material.color = col;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnGUI()
    {
        GUI.color = Color.black;
        if (_active)
        {
            Vector2 pos = cam.WorldToScreenPoint(new Vector2(leftDoor.transform.position.x + 3f, leftDoor.transform.position.y + 1.3f));
            GUI.Label(new Rect(pos.x, Screen.height - pos.y, 100, 20), Mathf.FloorToInt(_timeToOpen).ToString(), style);
        }
    }

    public void SetActive(bool active)
    {
        _active = active;
    }

}
