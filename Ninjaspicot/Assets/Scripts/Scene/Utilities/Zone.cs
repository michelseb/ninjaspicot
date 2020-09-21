using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Zone : MonoBehaviour
{
    private List<IActivable> _activables;
    private Light2D _ambiantLight;
    private ZoneManager _zoneManager;
    private float _lightIntensity;

    private void Awake()
    {
        _ambiantLight = GetComponent<Light2D>();
        _activables = GetComponentsInChildren<IActivable>().ToList();
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
        SetItemsActivation(true);
        StopAllCoroutines();
        StartCoroutine(OpenZone());
    }

    public void StartClose()
    {
        SetItemsActivation(false);
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

    private void SetItemsActivation(bool active)
    {
        for (int i = 0; i < _activables.Count; i++)
        {
            if (Utils.IsNull(_activables[i]))
            {
                _activables.RemoveAt(i);
                continue;
            }

            if (active)
            {
                _activables[i].Activate();
            }
            else
            {
                _activables[i].Deactivate();
            }
        }
    }
}
