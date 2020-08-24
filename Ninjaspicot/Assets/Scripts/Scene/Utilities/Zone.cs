using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Zone : MonoBehaviour
{
    private Light2D _ambiantLight;
    private ZoneManager _zoneManager;
    private float _lightIntensity;

    private void Awake()
    {
        _ambiantLight = GetComponent<Light2D>();
        _lightIntensity = _ambiantLight.intensity;
        Close();
    }

    private void Start()
    {
        _zoneManager = ZoneManager.Instance;
        _zoneManager.AddZone(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        _zoneManager.SetZone(this);
    }

    public void StartOpen()
    {
        StopAllCoroutines();
        StartCoroutine(OpenZone());
    }

    public void StartClose()
    {
        StopAllCoroutines();
        StartCoroutine(CloseZone());
    }

    private IEnumerator OpenZone()
    {
        _ambiantLight.intensity = 0;
        _ambiantLight.enabled = true;
        while (_ambiantLight.intensity < _lightIntensity)
        {
            _ambiantLight.intensity += Time.deltaTime;
            yield return null;
        }

        _ambiantLight.intensity = _lightIntensity;
    }

    private IEnumerator CloseZone()
    {
        while(_ambiantLight.intensity > 0)
        {
            _ambiantLight.intensity -= Time.deltaTime;
            yield return null;
        }

        Close();
    }

    private void Close()
    {
        _ambiantLight.enabled = false;
    }
}
