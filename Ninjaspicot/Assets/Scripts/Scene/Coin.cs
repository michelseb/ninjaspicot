using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int _value;

    private CharacterManager _characterManager;

    private void Awake()
    {
        _characterManager = CharacterManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _characterManager.Gain(_value);
        Destroy(gameObject);
    }
}
