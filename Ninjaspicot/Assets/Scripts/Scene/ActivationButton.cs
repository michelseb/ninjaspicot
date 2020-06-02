using UnityEngine;

public class ActivationButton : MonoBehaviour
{
    [SerializeField] private GameObject _activableObject;
    //[SerializeField] private CircleCollider2D _perimeter;
    public bool Pressing { get; set; }

    private IActivable _activable;
    private bool _active;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _activable = _activableObject.GetComponent<IActivable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") && !Pressing)
        {
            SetActive(ref _active);
            Pressing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Pressing = false;
    }

    private void SetActive(ref bool active)
    {
        active = !active;
        _renderer.color = active ? Color.green : Color.red;
        _activable.SetActive(active);
    }
}
