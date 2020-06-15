using UnityEngine;
using UnityEngine.UI;

public class Aim : MonoBehaviour, IRaycastable
{
    [SerializeField] private float _size;
    public float Size => _size;
    private Turret _turret;
    public Turret Turret { get { if (_turret == null) _turret = GetComponentInParent<Turret>() ?? GetComponentInChildren<Turret>(); return _turret; } }
    public int Id => Turret.Id;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    private void Update()
    {
        _image.enabled = Turret.Active;
    }

    private void Start()
    {
        transform.localScale = Vector3.one * _size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Turret.Active)
            return;

        if (collision.CompareTag("hero"))
        {
            var hit = Utils.LineCast(Turret.gameObject.transform.position, collision.gameObject.transform.position, Turret.Id);

            if (hit.transform.CompareTag("hero"))
            {
                if (!Turret.AutoShoot)
                {
                    Turret.StartAim(hit.transform);
                }
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!Turret.Active)
            return;

        if (collision.CompareTag("hero"))
        {
            if (!Turret.AutoShoot)
            {
                Turret.StartSearch();
            }
        }
    }
}
