using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int _order;
    public bool Attained { get; private set; }
    public int Order => _order;
    private Cloth _cloth;
    private SkinnedMeshRenderer _mesh;

    private void Awake()
    {
        _cloth = GetComponentInChildren<Cloth>();
        _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Attained && collision.CompareTag("hero"))
        {
            Attained = true;
            _cloth.externalAcceleration = Vector3.left * 20;
            _mesh.material.SetColor("_EmisColor", new Color(0, .7f, 0));
        }
    }
}
