using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleWidthCam : MonoBehaviour {

    public int targetWidth = 640;
    public float pixelsToUnits = 100;

	void Update () {

        int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
        GetComponent<Camera>().orthographicSize = height / pixelsToUnits / 2;

	}
}
