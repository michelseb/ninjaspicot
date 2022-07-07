using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{

    private static LightManager _instance;
    public static LightManager Instance { get { if (_instance == null) _instance = FindObjectOfType<LightManager>(); return _instance; } }

    public Light2D[] Lights { get; private set; }

    //private void Start()
    //{
    //    CalculateLights();
    //}

    public void CalculateLights()
    {
        Lights = FindObjectsOfType<Light2D>();
    }

    public void SetLightsActivation(bool active)
    {
        if (Lights == null)
        {
            CalculateLights();
        }

        foreach (var light in Lights)
        {
            if (light == null)
            {
                CalculateLights();
            }

            light.enabled = active;
        }
    }
}