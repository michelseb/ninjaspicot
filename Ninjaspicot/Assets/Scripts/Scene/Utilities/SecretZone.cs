using UnityEngine;

public class SecretZone : Zone
{
    private AudioSource _audioSource;
    private AudioManager _audioManager;
    private Audio _discoverSound;
    private bool _discovered;

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

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        
        if (!collision.CompareTag("hero"))
            return;

        if (!_discovered)
        {
            _audioManager.PlaySound(_audioSource, _discoverSound, .3f);
            _discovered = true;
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("hero"))
            return;

        ZoneManager.SetZone(this, false);
        Exited = false;
    }

    protected override void SetSpawn() { }
}
