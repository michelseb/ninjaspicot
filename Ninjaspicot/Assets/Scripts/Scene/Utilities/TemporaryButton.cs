using TMPro;
using UnityEngine;

public class TemporaryButton : ActivationButton
{
    [SerializeField] private float _activeTime;
    private float _remainingTime;
    private TextMeshPro _text;

    protected override void Awake()
    {
        base.Awake();
        _text = GetComponentInChildren<TextMeshPro>();
    }


    private void Update()
    {
        if (_remainingTime > 0)
        {
            if (!Pressing)
            {
                _remainingTime -= Time.deltaTime;
            }
            _text.text = _remainingTime.ToString("0.00");
        }
        else if (_active)
        {
            SetActive(false);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (Pressed(collision) && !Pressing)
        {
            _remainingTime = _activeTime;
            _audioManager.PlaySound(_audioSource, "Bip", .3f);
            SetActive(true);
            Pressing = true;
        }
    }

    protected override void SetActive(bool active)
    {
        base.SetActive(active);
        _text.enabled = active;
        _text.text = _remainingTime.ToString("0.00");
    }
}
