using UnityEngine;

public class Door : MonoBehaviour, IActivable
{
    [SerializeField] private bool _startOpened;
    
    private bool _opened;
    private Animator _animator;
    private ParticleSystem _dust;
    private AudioSource _audioSource;
    private AudioManager _audioManager;
    private Audio _bang;
    private Audio _open;
    private RepulsiveObstacle[] _doors;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _dust = GetComponentInChildren<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
        _audioManager = AudioManager.Instance;
        _doors = GetComponentsInChildren<RepulsiveObstacle>();
    }

    private void Start()
    {
        _bang = _audioManager.FindAudioByName("DoorBang");
        _open = _audioManager.FindAudioByName("DoorOpen");
        if (_startOpened)
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (_opened)
            return;

        _opened = true;
        _animator.SetTrigger("Open");
        
        foreach (var door in _doors)
        {
            door.DetachHero();
        }
    }

    public void Deactivate()
    {
        if (!_opened)
            return;

        _opened = false;
        _animator.SetTrigger("Close");
    }

    public void Open()
    {
        _audioManager.PlaySound(_audioSource, _open, .3f);
    }

    public void Dust()
    {
        _dust.Play();
        _audioManager.PlaySound(_audioSource, _bang, .5f);
    }
}
