using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Zone : MonoBehaviour
{
    private List<IWakeable> _wakeables;
    private Light2D _ambiantLight;
    private ZoneManager _zoneManager;
    private float _lightIntensity;
    private Animator _animator;

    private long _id;
    public long Id { get { if (_id == 0) _id = GetInstanceID(); return _id; } }

    public bool Exited { get; private set; }

    private void Awake()
    {
        _ambiantLight = GetComponent<Light2D>();
        _lightIntensity = _ambiantLight.intensity;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _wakeables = GetComponentsInChildren<IWakeable>().ToList();
        Close();
        _zoneManager = ZoneManager.Instance;
        _zoneManager.AddZone(this);
        Exited = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        Exited = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        _zoneManager.SetZone(this);
        Exited = false;
    }

    public void Open()
    {
        SetItemsActivation(true);

        _animator.SetTrigger("Open");
        //StopAllCoroutines();
        //StartCoroutine(OpenZone());
    }

    public void Close()
    {
        SetItemsActivation(false);

        _animator.SetTrigger("Close");
        //StopAllCoroutines();
        //StartCoroutine(CloseZone());
    }

    //private IEnumerator OpenZone()
    //{
    //    _ambiantLight.intensity = 0;
    //    _ambiantLight.enabled = true;
    //    while (_ambiantLight.intensity < _lightIntensity)
    //    {
    //        _ambiantLight.intensity += Time.deltaTime;
    //        yield return null;
    //    }

    //    _ambiantLight.intensity = _lightIntensity;
    //}

    //private IEnumerator CloseZone()
    //{
    //    while(_ambiantLight.intensity > 0)
    //    {
    //        _ambiantLight.intensity -= Time.deltaTime;
    //        yield return null;
    //    }

    //    Close();
    //}

    //private void Close()
    //{
    //    _ambiantLight.enabled = false;
    //}

    private void SetItemsActivation(bool active)
    {
        for (int i = 0; i < _wakeables.Count; i++)
        {
            if (Utils.IsNull(_wakeables[i]))
            {
                _wakeables.RemoveAt(i);
                continue;
            }

            if (active)
            {
                _wakeables[i].Wake();
            }
            else
            {
                _wakeables[i].Sleep();
            }
        }
    }
}
