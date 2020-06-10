using UnityEngine;

public class Coin : Bonus
{
    [SerializeField] private int _value;

    private CharacterManager _characterManager;

    protected override void Awake()
    {
        base.Awake();
        _characterManager = CharacterManager.Instance;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero"))
        {
            _characterManager.Gain(_value);
        }

        base.OnTriggerEnter2D(collision);
    }
}
