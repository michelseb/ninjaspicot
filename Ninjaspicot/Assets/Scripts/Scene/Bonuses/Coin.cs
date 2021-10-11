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

    public override void Take()
    {
        _characterManager.Gain(_value);
        _audioManager.PlaySound(_audioSource, "Coin");

        base.Take();
    }

    public override void DoReset()
    {
        _taken = false;
        Activate();
        base.DoReset();
    }
}
