using UnityEngine;

public class ActivationButton : MonoBehaviour
{
    [SerializeField] protected bool _active;
    [SerializeField] protected GameObject _activableObject;
    public bool Pressing { get; set; }

    protected IActivable _activable;
    protected SpriteRenderer _renderer;

    protected virtual void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        _activable = _activableObject.GetComponent<IActivable>();
        SetActive(_active);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("hero") && !Pressing)
        {
            ToggleActivation(_active);
            Pressing = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        Pressing = false;
    }

    protected virtual void ToggleActivation(bool active)
    {
        active = !active;
        SetActive(active);
    }

    protected virtual void SetActive(bool active)
    {
        _active = active;
        _renderer.color = active ? Color.red : Color.green;

        if (active)
        {
            _activable.Activate();
        }
        else
        {
            _activable.Deactivate();
        }
    }
}
