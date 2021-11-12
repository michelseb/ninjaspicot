using UnityEngine;

public class SecretZone : Zone
{
    private AudioSource _audioSource;
    private AudioManager _audioManager;
    private Audio _discoverSound;
    private bool _discovered;
    private bool _opened;

    protected override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
    }

    protected override void Start()
    {
        base.Start();
        _discoverSound = _audioManager.FindAudioByName("Discover");
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        OnTriggerStay2D(collision);
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (_opened || !collision.CompareTag("hero"))
            return;

        if (!_discovered)
        {
            _audioManager.PlaySound(_audioSource, _discoverSound, .3f);
            _discovered = true;
        }

        ZoneManager.OpenExtraZone(this);
        _opened = true;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        Close();
    }

    public override void Close()
    {
        base.Close();

        _opened = false;
        _zoneManager.UpdateCurrentZoneCameraBehavior();
    }

    protected override void SetSpawn() { }
}
