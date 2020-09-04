using UnityEngine;

public class TemporaryButton : ActivationButton
{
    [SerializeField] private float _activeTime;
    private float _remainingTime;


    private void Update()
    {
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
        }
        else if (_active)
        {
            SetActive(false);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") && !Pressing)
        {
            _remainingTime = _activeTime;
            _audioManager.PlaySound(_audioSource, "Bip", .3f);
            SetActive(true);
            Pressing = true;
        }
    }
}
