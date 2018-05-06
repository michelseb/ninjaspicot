using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWidthCam : MonoBehaviour {

    public static int targetWidth = 1080;
    public static float pixelsToUnits = 20;

	public void Awake() {

        int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
        GetComponent<Camera>().orthographicSize = height / pixelsToUnits / 2;

	}
}
