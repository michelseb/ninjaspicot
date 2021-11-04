using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int _order;
    public bool Attained { get; private set; }
    public int Order => _order;

    public void Attain()
    {
        Attained = true;
    }
}
