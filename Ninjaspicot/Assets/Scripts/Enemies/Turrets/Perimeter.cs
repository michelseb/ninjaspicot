using UnityEngine;

public class Perimeter : MonoBehaviour
{
    [SerializeField] private float _size;
    public float Size => _size;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        _transform.localScale = Vector3.one * Size;
    }
}
