using UnityEngine;

public class Coin : Bonus
{
    [SerializeField] private int _value;

    private CharacterManager _characterManager;
    private AudioSource _audioSource;
    private AudioManager _audioManager;

    protected override void Awake()
    {
        base.Awake();
        _characterManager = CharacterManager.Instance;
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            _characterManager.Gain(_value);
            _audioManager.PlaySound(_audioSource, "Coin");
        }

        base.OnTriggerEnter2D(collision);
    }
}
