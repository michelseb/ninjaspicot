﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    public GameObject ninja;
    private Camera cam;

    public bool centerX;
    private float leftScreen = Screen.width / 6;
    private float rightScreen = Screen.width * 5 / 6;
    private float topScreen = Screen.height / 6;
    private float bottomScreen = Screen.height * 5 / 6;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        Center();
        //CheckPositionOnScreen(ninja.transform.position);
        leftScreen = Screen.width / 6;
        rightScreen = Screen.width * 5 / 6;
        topScreen = Screen.height / 6;
        bottomScreen = Screen.height * 5 / 6;
    }

    private void CheckPositionOnScreen(Vector2 pos)
    {
        Vector3 left = cam.ScreenToWorldPoint(new Vector3(leftScreen, 0, 0));
        Vector3 right = cam.ScreenToWorldPoint(new Vector3(rightScreen, 0, 0));
        Vector3 top = cam.ScreenToWorldPoint(new Vector3(topScreen, 0, 0));
        Vector3 bottom = cam.ScreenToWorldPoint(new Vector3(bottomScreen, 0, 0));
        if (pos.x < left.x || pos.x > right.x || pos.y < top.x || pos.y > bottom.x)
        {
            Center();
        }
    }

    private void Center()
    {
        if (centerX)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, ninja.transform.position.y, transform.position.z), .1f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(ninja.transform.position.x, ninja.transform.position.y, transform.position.z), .1f);
        }
    }

    public IEnumerator zoomIn(int zoom)
    {
        while (cam.orthographicSize > zoom)
        {
            
            cam.orthographicSize--;
            yield return null;
        }
        
    }

    public IEnumerator zoomOut(int zoom)
    {
        while (cam.orthographicSize < zoom)
        {
            cam.orthographicSize++;
            yield return null;
        }

    }

}
