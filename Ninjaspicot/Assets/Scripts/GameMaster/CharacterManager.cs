using UnityEngine;



public class CharacterManager : MonoBehaviour
{
    public int Money { get; private set; }

    private static CharacterManager _instance;
    public static CharacterManager Instance { get { if (_instance == null) _instance = FindObjectOfType<CharacterManager>(); return _instance; } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Gain(int value)
    {
        Money += value;
    }

    public bool Spend(int value)
    {
        if (value > Money)
            return false;

        Money -= value;
        return true;
    }
}
